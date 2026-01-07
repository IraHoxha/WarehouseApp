using AutoMapper;
using Microsoft.EntityFrameworkCore;
using warehouse.Data;
using warehouseapp.Data.Models;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels;
using warehouseapp.ViewModels.Category;

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
        private static string NormalizeName(string name)
        {
            return new string(
                name.Trim()
                    .ToUpperInvariant()
                    .Where(char.IsLetterOrDigit)
                    .ToArray()
            );
        }

        public async Task<CategoryResponseViewModel> CreateAsync(CategoryRequestViewModel model)
        {
            model.Name = model.Name?.Trim();

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ValidationException("Category name is required.");

            if (model.ParentCategoryId.HasValue &&
                !await _context.Categories.AnyAsync(c => c.Id == model.ParentCategoryId))
                throw new NotFoundException("Parent category not found.");

            var normalized = NormalizeName(model.Name);

            var existingNames = await _context.Categories
                .Select(c => c.Name)
                .ToListAsync();

            bool exists = existingNames
                .Any(name => NormalizeName(name) == normalized);

            if (exists)
                throw new ValidationException("Category with the same name already exists.");

            var category = new Category
            {
                Name = model.Name,
                ParentCategoryId = model.ParentCategoryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryResponseViewModel>(category);
        }

        public async Task<CategoryResponseViewModel> UpdateAsync(int id, CategoryRequestViewModel model)
        {
            model.Name = model.Name?.Trim();

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ValidationException("Category name is required.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new NotFoundException("Category not found.");

            if (model.ParentCategoryId == id)
                throw new ValidationException("Category cannot be its own parent.");

            var normalized = NormalizeName(model.Name);

            var existingNames = await _context.Categories
                .Where(c => c.Id != id)
                .Select(c => c.Name)
                .ToListAsync();

            bool exists = existingNames
                .Any(name => NormalizeName(name) == normalized);

            if (exists)
                throw new ValidationException("Category with the same name already exists.");

            category.Name = model.Name;
            category.ParentCategoryId = model.ParentCategoryId;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryResponseViewModel>(category);
        }

        public async Task DeleteAsync(int id)
        {
            if (await _context.Categories.AnyAsync(c => c.ParentCategoryId == id))
                throw new ValidationException("Cannot delete category with subcategories.");

            if (await _context.Products.AnyAsync(p => p.CategoryId == id))
                throw new ValidationException("Cannot delete category with products.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new NotFoundException("Category not found.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<CategoryResponseViewModel> GetByIdAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new NotFoundException("Category not found.");

            return _mapper.Map<CategoryResponseViewModel>(category);
        }

        public async Task<List<CategoryResponseViewModel>> GetAllAsync()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return _mapper.Map<List<CategoryResponseViewModel>>(categories);
        }

        public async Task<List<CategorySelectItemViewModel>> GetAllForSelectAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategorySelectItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId
                })
                .ToListAsync();
        }
        
        public async Task<List<CategorySelectItemViewModel>> GetLeafForSelectAsync()
        {
            var parentIds = await _context.Categories
                .Where(c => c.ParentCategoryId != null)
                .Select(c => c.ParentCategoryId!.Value)
                .Distinct()
                .ToListAsync();

            return await _context.Categories
                .Where(c => !parentIds.Contains(c.Id))
                .OrderBy(c => c.Name)
                .Select(c => new CategorySelectItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId
                })
                .ToListAsync();
        }
    }
}
