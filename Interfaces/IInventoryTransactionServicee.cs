using warehouseapp.ViewModels.Inventory;

namespace warehouseapp.Interfaces
{
    public interface IInventoryTransactionService
    {
        Task<List<InventoryTransactionResponseViewModel>> GetAllAsync();
    }
}
