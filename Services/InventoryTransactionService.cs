using AutoMapper;
using Microsoft.EntityFrameworkCore;
using warehouse.Data;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Inventory;

namespace warehouseapp.Services
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        private static readonly int RetentionDays = 30;

        public InventoryTransactionService(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<InventoryTransactionResponseViewModel>> GetAllAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-RetentionDays);

            var transactions = await _context.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.Partner)
                .Include(t => t.Order)
                .Where(t =>
                    !t.CompletedAt.HasValue ||
                    t.CompletedAt >= cutoff
                )
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<InventoryTransactionResponseViewModel>>(transactions);
        }

    }
}
