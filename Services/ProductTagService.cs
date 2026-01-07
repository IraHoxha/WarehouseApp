using Microsoft.EntityFrameworkCore;
using warehouse.Data;
using warehouseapp.Data.Models;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Tag;

namespace warehouseapp.Services
{
    public class ProductTagService : IProductTagService
    {
        private readonly WarehouseDbContext _context;

        public ProductTagService(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductTagDisplayViewModel>> GetByProductIdAsync(int productId)
        {
            if (!await _context.Products.AnyAsync(p => p.Id == productId))
                throw new NotFoundException("Product not found.");

            return await _context.ProductTags
                .Include(pt => pt.Tag)
                .Include(pt => pt.TagValue)
                .Where(pt => pt.ProductId == productId)
                .OrderBy(pt => pt.Tag.Key)
                .Select(pt => new ProductTagDisplayViewModel
                {
                    Id = pt.Id,
                    TagId = pt.TagId,
                    TagKey = pt.Tag.Key,
                    TagValueId = pt.TagValueId,
                    Value = pt.TagValue.Value
                })
                .ToListAsync();
        }

        public async Task<ProductTagDisplayViewModel> AddAsync(ProductTagRequestViewModel model)
        {
            if (!await _context.Products.AnyAsync(p => p.Id == model.ProductId))
                throw new NotFoundException("Product not found.");

            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == model.TagId)
                ?? throw new NotFoundException("Tag not found.");

            if (string.IsNullOrWhiteSpace(model.Value))
                throw new ValidationException("Value is required.");

            var normalized = model.Value.Trim();

            var tagValue = await _context.TagValues
                .FirstOrDefaultAsync(v =>
                    v.TagId == model.TagId &&
                    v.Value.ToLower() == normalized.ToLower());

            if (tagValue == null)
            {
                tagValue = new TagValue
                {
                    TagId = model.TagId,
                    Value = normalized
                };

                _context.TagValues.Add(tagValue);
                await _context.SaveChangesAsync();
            }

            var exists = await _context.ProductTags.AnyAsync(pt =>
                pt.ProductId == model.ProductId &&
                pt.TagId == model.TagId &&
                pt.TagValueId == tagValue.Id);

            if (exists)
                throw new ValidationException("Tag already added for this product.");

            var productTag = new ProductTag
            {
                ProductId = model.ProductId,
                TagId = model.TagId,
                TagValueId = tagValue.Id
            };

            _context.ProductTags.Add(productTag);
            await _context.SaveChangesAsync();

            return new ProductTagDisplayViewModel
            {
                Id = productTag.Id, 
                TagId = tag.Id,
                TagKey = tag.Key,
                TagValueId = tagValue.Id,
                Value = tagValue.Value
            };
        }

        public async Task DeleteAsync(int id)
        {
            var pt = await _context.ProductTags
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException("Product tag not found.");

            _context.ProductTags.Remove(pt);
            await _context.SaveChangesAsync();
        }
    }
}
