using AutoMapper;
using Microsoft.EntityFrameworkCore;
using warehouse.Data;
using warehouseapp.Data.Models;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Product;

namespace WarehouseApp.Services
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

        public async Task<ProductResponseViewModel> CreateProductAsync(ProductRequestViewModel request)
        {
            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                throw new NotFoundException("Category not found.");

            bool skuExists = await _context.Products
                .AnyAsync(p => p.SKU == request.SKU);
            if (skuExists)
                throw new ValidationException("A product with this SKU already exists.");
            var product = _mapper.Map<Product>(request);
            product.QuantityInStock = 0;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            product = await _context.Products
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == product.Id);

            return _mapper.Map<ProductResponseViewModel>(product);
        }

        public async Task<ProductResponseViewModel> UpdateProductAsync(int id, ProductRequestViewModel request)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            if (product.SKU != request.SKU)
            {
                bool skuExists = await _context.Products
                    .AnyAsync(p => p.SKU == request.SKU && p.Id != id);

                if (skuExists)
                    throw new ValidationException("A product with this SKU already exists.");
            }

            _mapper.Map(request, product);
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<ProductResponseViewModel>(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new NotFoundException("Product not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            return product;
        }

        public async Task<List<ProductResponseViewModel>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return _mapper.Map<List<ProductResponseViewModel>>(products);
        }

        public async Task<bool> UpdateStockAsync(int productId, decimal quantityChange)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found.");

            product.QuantityInStock += quantityChange;

            if (product.QuantityInStock < 0)
                throw new ValidationException("Stock level cannot be negative.");

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
