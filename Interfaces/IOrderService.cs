using warehouseapp.ViewModels.Order;

namespace warehouseapp.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseViewModel> CreateAsync(OrderRequestViewModel model);

        Task ProcessAsync(int id);
        Task CompleteAsync(int id);
        Task CancelAsync(int id);

        Task ReturnAsync(int orderId, OrderReturnRequestViewModel model);

        Task<OrderResponseViewModel> GetByIdAsync(int id);
        Task<List<OrderResponseViewModel>> GetAllAsync();
    }
}
