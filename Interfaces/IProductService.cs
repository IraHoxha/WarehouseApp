using warehouseapp.ViewModels.Product;

namespace warehouseapp.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseViewModel> CreateProductAsync(ProductRequestViewModel model);
        Task<ProductResponseViewModel> UpdateProductAsync(int id, ProductRequestViewModel model);
        Task<bool> DeleteProductAsync(int id);

        Task<ProductResponseViewModel> GetProductByIdAsync(int id);
        Task<ProductEditResponseViewModel> GetProductForEditAsync(int id);
        Task<List<ProductResponseViewModel>> GetAllProductsAsync();

        Task<List<ProductResponseViewModel>> FilterAsync(
            string? search,
            int? categoryId,
            bool? hasExpiration);

        Task<bool> UpdateStockAsync(int productId, int quantityChange);

        Task<List<ProductSelectItemViewModel>> GetForSelectAsync();
    }
}
