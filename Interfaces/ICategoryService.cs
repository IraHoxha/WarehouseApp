using warehouseapp.ViewModels;
using warehouseapp.ViewModels.Category;

namespace warehouseapp.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseViewModel> CreateAsync(CategoryRequestViewModel request);
        Task<CategoryResponseViewModel> UpdateAsync(int id, CategoryRequestViewModel request);
        Task<bool> DeleteAsync(int id);
        Task<CategoryResponseViewModel> GetByIdAsync(int id);
        Task<List<CategoryResponseViewModel>> GetAllAsync();
    }
}
