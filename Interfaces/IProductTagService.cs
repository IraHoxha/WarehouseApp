using warehouseapp.ViewModels.Tag;

namespace warehouseapp.Interfaces
{
    public interface IProductTagService
    {
        Task<List<ProductTagDisplayViewModel>> GetByProductIdAsync(int productId);
        Task<ProductTagDisplayViewModel> AddAsync(ProductTagRequestViewModel model);
        Task DeleteAsync(int id);
    }
}
