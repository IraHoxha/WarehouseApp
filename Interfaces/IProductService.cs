using warehouseapp.Data.Models;
using warehouseapp.ViewModels;

namespace warehouseapp.Interfaces{
    public interface IProductService
    {
        Task<ProductResponseViewModel> CreateProductAsync(ProductRequestViewModel request);
        Task<ProductResponseViewModel> UpdateProductAsync(int id, ProductRequestViewModel request);
        Task<bool> DeleteProductAsync(int id);
        Task<Product> GetProductByIdAsync(int id);  
        Task<List<ProductResponseViewModel>> GetAllProductsAsync();
        Task<bool> UpdateStockAsync(int productId, decimal quantityChange);
    }
}
