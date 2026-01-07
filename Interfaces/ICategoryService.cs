using warehouseapp.ViewModels;
using warehouseapp.ViewModels.Category;

namespace warehouseapp.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseViewModel> CreateAsync(CategoryRequestViewModel model);
        Task<CategoryResponseViewModel> UpdateAsync(int id, CategoryRequestViewModel model);
        Task DeleteAsync(int id);

        Task<CategoryResponseViewModel> GetByIdAsync(int id);
        Task<List<CategoryResponseViewModel>> GetAllAsync();
        Task<List<CategorySelectItemViewModel>> GetAllForSelectAsync();
        Task<List<CategorySelectItemViewModel>> GetLeafForSelectAsync();
    }
}
