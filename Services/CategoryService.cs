using AutoMapper;
using Microsoft.EntityFrameworkCore;
using warehouse.Data;
using warehouseapp.Data.Models;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Category;
using warehouseapp.ViewModels;

namespace warehouseapp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CategoryResponseViewModel> CreateAsync(CategoryRequestViewModel request)
        {
            if (request.ParentCategoryId.HasValue)
            {
                bool parentExists = await _context.Categories
                    .AnyAsync(c => c.Id == request.ParentCategoryId);
                if (!parentExists)
                    throw new NotFoundException("Parent category not found.");
            }

            var category = _mapper.Map<Category>(request);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstAsync(c => c.Id == category.Id);

            return _mapper.Map<CategoryResponseViewModel>(category);
        }

        public async Task<CategoryResponseViewModel> UpdateAsync(int id, CategoryRequestViewModel request)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new NotFoundException("Category not found.");

            if (request.ParentCategoryId == id)
                throw new ValidationException("A category cannot be its own parent.");

            if (request.ParentCategoryId.HasValue)
            {
                bool parentExists = await _context.Categories
                    .AnyAsync(c => c.Id == request.ParentCategoryId);
                if (!parentExists)
                    throw new NotFoundException("Parent category not found.");
            }

            _mapper.Map(request, category);
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryResponseViewModel>(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new NotFoundException("Category not found.");

            if (category.SubCategories.Any())
                throw new ValidationException("Cannot delete a category that has subcategories.");

            if (category.Products.Any())
                throw new ValidationException("Cannot delete a category that has products assigned.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<CategoryResponseViewModel> GetByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new NotFoundException("Category not found.");

            return _mapper.Map<CategoryResponseViewModel>(category);
        }

        public async Task<List<CategoryResponseViewModel>> GetAllAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.SubCategories)
                .ToListAsync();

            return _mapper.Map<List<CategoryResponseViewModel>>(categories);
        }
    }
}
