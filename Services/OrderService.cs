using AutoMapper;
using Microsoft.EntityFrameworkCore;
using warehouse.Data;
using warehouseapp.Data.Models;
using warehouseapp.Enums;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Order;

namespace warehouseapp.Services
{
    public class OrderService : IOrderService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        private static readonly int MinPurchaseShelfLifeDays = 30;
        private static readonly int RetentionDays = 30;

        public OrderService(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private static bool ApplyExpiration(Order order)
        {
            if (order.Status == OrderStatusEnum.Pending &&
                order.ExpirationDate.Date < DateTime.Today)
            {
                order.Status = OrderStatusEnum.Expired;
                return true;
            }

            return false;
        }

        public async Task<OrderResponseViewModel> CreateAsync(OrderRequestViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
                throw new ValidationException("Order must contain items.");

            if (model.ExpirationDate.Date < DateTime.Today)
                throw new ValidationException("Order expiration date cannot be in the past.");

            var partner = await _context.Partners.FindAsync(model.PartnerId)
                ?? throw new NotFoundException("Partner not found.");

            if (model.Type == OrderTypeEnum.Sale && partner.Type != PartnerTypeEnum.Customer)
                throw new ValidationException("Sales orders require a customer.");

            if (model.Type == OrderTypeEnum.Purchase && partner.Type != PartnerTypeEnum.Supplier)
                throw new ValidationException("Purchase orders require a supplier.");

            var minExpiry = DateTime.Today.AddDays(MinPurchaseShelfLifeDays);

            if (model.Type == OrderTypeEnum.Purchase)
            {
                bool duplicateBatch = model.Items
                    .Where(i => !string.IsNullOrWhiteSpace(i.BatchNumber))
                    .GroupBy(i => i.BatchNumber)
                    .Any(g => g.Count() > 1);

                if (duplicateBatch)
                    throw new ValidationException("Duplicate batch numbers are not allowed in a purchase order.");
            }

            foreach (var item in model.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId)
                    ?? throw new NotFoundException("Product not found.");

                if (model.Type == OrderTypeEnum.Sale && product.QuantityInStock < item.Quantity)
                {
                    throw new ValidationException(
                        $"Insufficient stock for '{product.Name}'. In stock: {product.QuantityInStock}");
                }

                if (model.Type == OrderTypeEnum.Purchase)
                {
                    if (string.IsNullOrWhiteSpace(item.BatchNumber))
                        throw new ValidationException("Batch number is required for purchase orders.");

                    if (!item.ExpirationDate.HasValue)
                        throw new ValidationException("Expiration date is required for purchase orders.");

                    if (item.ExpirationDate.Value.Date < minExpiry)
                        throw new ValidationException(
                            $"Cannot purchase items expiring within {MinPurchaseShelfLifeDays} days.");
                }
            }

            var order = new Order
            {
                PartnerId = partner.Id,
                Partner = partner,
                Type = model.Type,
                ExpirationDate = model.ExpirationDate,
                Status = OrderStatusEnum.Pending
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in model.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId)!;

                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Product = product,
                    Quantity = item.Quantity,
                    UnitSellingPrice = item.UnitSellingPrice,
                    BatchNumber = model.Type == OrderTypeEnum.Purchase ? item.BatchNumber : null,
                    ExpirationDate = model.Type == OrderTypeEnum.Purchase ? item.ExpirationDate : null
                });
            }

            await _context.SaveChangesAsync();
            return await GetByIdAsync(order.Id);
        }

        public async Task ProcessAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id)
                ?? throw new NotFoundException("Order not found.");

            ApplyExpiration(order);

            if (order.Status != OrderStatusEnum.Pending)
                throw new ValidationException("Order cannot be processed.");

            order.Status = OrderStatusEnum.Processing;
            await _context.SaveChangesAsync();
        }


        public async Task CompleteAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .Include(o => o.Partner)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new NotFoundException("Order not found.");

            ApplyExpiration(order);

            if (order.Status != OrderStatusEnum.Processing)
                throw new ValidationException("Order cannot be completed.");

            foreach (var item in order.OrderItems)
            {
                var txType = order.Type == OrderTypeEnum.Sale
                    ? TransactionTypeEnum.Out
                    : TransactionTypeEnum.In;

                item.Product.QuantityInStock +=
                    txType == TransactionTypeEnum.In
                        ? item.Quantity
                        : -item.Quantity;

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductId = item.ProductId,
                    Product = item.Product,
                    PartnerId = order.PartnerId,
                    Partner = order.Partner,
                    OrderId = order.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitSellingPrice, 
                    TransactionType = txType,
                    Source = InventoryTransactionSourceEnum.OrderFulfillment,
                    BatchNumber = order.Type == OrderTypeEnum.Purchase ? item.BatchNumber : null,
                    ExpirationDate = order.Type == OrderTypeEnum.Purchase ? item.ExpirationDate : null,
                    CompletedAt = DateTime.UtcNow
                });
            }

            order.Status = OrderStatusEnum.Completed;
            order.CompletedAt = DateTime.UtcNow; 
            await _context.SaveChangesAsync();
        }


        public async Task ReturnAsync(int orderId, OrderReturnRequestViewModel model)
        {
            var order = await _context.Orders
                .Include(o => o.Partner)
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new NotFoundException("Order not found.");

            if (order.Status != OrderStatusEnum.Completed)
                throw new ValidationException("Only completed orders can be returned.");

            var item = order.OrderItems
                .FirstOrDefault(i => i.ProductId == model.ProductId)
                ?? throw new NotFoundException("Order item not found.");

            if (model.Quantity <= 0 || model.Quantity > item.Quantity)
                throw new ValidationException("Invalid return quantity.");

            if (model.ReturnReason == null)
                throw new ValidationException("Return reason is required.");

            item.Quantity -= model.Quantity;
            item.Product.QuantityInStock += model.Quantity;

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductId = item.ProductId,
                Product = item.Product,
                PartnerId = order.PartnerId,
                OrderId = order.Id,
                Partner = order.Partner,
                Quantity = model.Quantity,
                UnitPrice = item.UnitSellingPrice, 
                TransactionType = TransactionTypeEnum.In,
                Source = InventoryTransactionSourceEnum.SalesReturn,
                ReturnReason = model.ReturnReason.Value.ToString(),
                CompletedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }


        public async Task<OrderResponseViewModel> GetByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Partner)
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new NotFoundException("Order not found.");

            if (ApplyExpiration(order))
                await _context.SaveChangesAsync();

            return _mapper.Map<OrderResponseViewModel>(order);
        }

        public async Task<List<OrderResponseViewModel>> GetAllAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-RetentionDays);

            var orders = await _context.Orders
                .Include(o => o.Partner)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                .Where(o =>
                    o.Status != OrderStatusEnum.Completed ||
                    (o.CompletedAt.HasValue && o.CompletedAt >= cutoff)
                )
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<OrderResponseViewModel>>(orders);
        }
    

        public async Task CancelAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id)
                ?? throw new NotFoundException("Order not found.");

            order.Status = OrderStatusEnum.Cancelled;
            await _context.SaveChangesAsync();
        }
    }
}
