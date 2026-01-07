using AutoMapper;
using Microsoft.EntityFrameworkCore;
using warehouse.Data;
using warehouseapp.Data.Models;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Product;

namespace warehouseapp.Services
{
    public class ProductService : IProductService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductResponseViewModel> CreateProductAsync(ProductRequestViewModel model)
        {
            if (!await _context.Categories.AnyAsync(c => c.Id == model.CategoryId))
                throw new NotFoundException("Category not found.");

            model.SKU = NormalizeSku(model.SKU);

            if (string.IsNullOrWhiteSpace(model.SKU))
                model.SKU = await GenerateSkuAsync(model.Name, model.CategoryId);

            if (await _context.Products.AnyAsync(p => p.SKU == model.SKU))
                throw new ValidationException("SKU already exists.");

            var product = _mapper.Map<Product>(model);
            product.QuantityInStock = 0;
            product.ReorderLevel = model.ReorderLevel;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id);
        }

        public async Task<ProductResponseViewModel> UpdateProductAsync(int id, ProductRequestViewModel model)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException("Product not found.");

            model.SKU = NormalizeSku(model.SKU);

            if (string.IsNullOrWhiteSpace(model.SKU))
                throw new ValidationException("SKU is required.");

            if (await _context.Products.AnyAsync(p =>
                p.SKU == model.SKU && p.Id != id))
            {
                throw new ValidationException("SKU already exists.");
            }

            _mapper.Map(model, product);
            product.ReorderLevel = model.ReorderLevel;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException("Product not found.");

            _context.Products.Remove(product);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                throw new ValidationException(
                    "Cannot delete product because it is used in orders or inventory transactions."
                );
            }
        }

        public async Task<ProductResponseViewModel> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.TagValue)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException("Product not found.");

            return _mapper.Map<ProductResponseViewModel>(product);
        }

        public async Task<List<ProductResponseViewModel>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.TagValue)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<ProductResponseViewModel>>(products);
        }

        public async Task<List<ProductResponseViewModel>> FilterAsync(
            string? search,
            int? categoryId,
            bool? hasExpiration)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.SKU.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (hasExpiration.HasValue)
                query = query.Where(p => p.HasExpiration == hasExpiration.Value);

            var list = await query
                .OrderBy(p => p.Name)
                .ToListAsync();

            return _mapper.Map<List<ProductResponseViewModel>>(list);
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantityChange)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new NotFoundException("Product not found.");

            var newStock = product.QuantityInStock + quantityChange;

            if (newStock < 0)
                throw new ValidationException("Stock cannot go below zero.");

            product.QuantityInStock = newStock;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductEditResponseViewModel> GetProductForEditAsync(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException("Product not found.");

            return new ProductEditResponseViewModel
            {
                Id = product.Id,
                SKU = product.SKU,
                Name = product.Name,
                Description = product.Description,
                UnitOfMeasurement = product.UnitOfMeasurement,
                CategoryId = product.CategoryId,
                UnitCostPrice = product.UnitCostPrice,
                UnitSellingPrice = product.UnitSellingPrice,
                HasExpiration = product.HasExpiration,
                ReorderLevel = product.ReorderLevel
            };
        }

        public async Task<List<ProductSelectItemViewModel>> GetForSelectAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.SKU)
                .Select(p => new ProductSelectItemViewModel
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Name = p.Name
                })
                .ToListAsync();
        }

        private async Task<string> GenerateSkuAsync(string name, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Product name is required to generate SKU.");

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categoryId)
                ?? throw new ValidationException("Category not found.");

            var cat = Normalize(category.Name, 3);
            var prod = Normalize(name, 20);

            return $"{cat}-{prod}";
        }

        private static string Normalize(string value, int maxLength)
        {
            var cleaned = new string(
                value.ToUpperInvariant()
                     .Where(char.IsLetterOrDigit)
                     .ToArray()
            );

            return cleaned.Length > maxLength
                ? cleaned[..maxLength]
                : cleaned;
        }

        private static string NormalizeSku(string? sku)
            => sku?.Trim().ToUpperInvariant() ?? string.Empty;
    }
}
