using Microsoft.EntityFrameworkCore;
using warehouse.Data;
using warehouseapp.Data.Models;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Tag;

namespace warehouseapp.Services
{
    public class TagService : ITagService
    {
        private readonly WarehouseDbContext _context;

        public TagService(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<TagResponseViewModel> CreateAsync(TagRequestViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Key))
                throw new ValidationException("Tag key is required.");

            var key = model.Key.Trim();

            if (await _context.Tags.AnyAsync(t => t.Key.ToLower() == key.ToLower()))
                throw new ValidationException("This tag already exists.");

            var tag = new Tag { Key = key };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return new TagResponseViewModel
            {
                Id = tag.Id,
                Key = tag.Key,
                Values = new()
            };
        }

        public async Task<List<TagResponseViewModel>> GetAllAsync()
        {
            var tags = await _context.Tags
                .Include(t => t.Values)
                .OrderBy(t => t.Key)
                .ToListAsync();

            return tags.Select(t => new TagResponseViewModel
            {
                Id = t.Id,
                Key = t.Key,
                Values = t.Values
                    .Select(v => v.Value)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(v => v)
                    .ToList()
            }).ToList();
        }

        public async Task<List<TagValueResponseViewModel>> GetValuesAsync(int tagId)
        {
            if (!await _context.Tags.AnyAsync(t => t.Id == tagId))
                throw new NotFoundException("Tag not found.");

            return await _context.TagValues
                .Where(v => v.TagId == tagId)
                .OrderBy(v => v.Value)
                .Select(v => new TagValueResponseViewModel
                {
                    Id = v.Id,
                    Value = v.Value
                })
                .ToListAsync();
        }

        public async Task DeleteIfUnusedAsync(int id)
        {
            var tag = await _context.Tags
                .Include(t => t.Values)
                .FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new NotFoundException("Tag not found.");

            if (await _context.ProductTags.AnyAsync(pt => pt.TagId == id))
                throw new ValidationException("Tag is used by products.");

            if (tag.Values.Any())
                throw new ValidationException("Tag has values.");

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTagValueIfUnusedAsync(int tagValueId)
        {
            var value = await _context.TagValues
                .FirstOrDefaultAsync(v => v.Id == tagValueId)
                ?? throw new NotFoundException("Tag value not found.");

            if (await _context.ProductTags.AnyAsync(pt => pt.TagValueId == tagValueId))
                throw new ValidationException("Tag value is used by products.");

            _context.TagValues.Remove(value);
            await _context.SaveChangesAsync();
        }

        public async Task CleanupAsync()
        {
            var unusedValues = await _context.TagValues
                .Where(v => !_context.ProductTags.Any(pt => pt.TagValueId == v.Id))
                .ToListAsync();

            if (unusedValues.Any())
            {
                _context.TagValues.RemoveRange(unusedValues);
                await _context.SaveChangesAsync();
            }

            var tagsWithoutValues = await _context.Tags
                .Include(t => t.Values)
                .Where(t => !t.Values.Any())
                .ToListAsync();

            if (tagsWithoutValues.Any())
            {
                _context.Tags.RemoveRange(tagsWithoutValues);
                await _context.SaveChangesAsync();
            }

            var unusedTags = await _context.Tags
                .Where(t => !_context.ProductTags.Any(pt => pt.TagId == t.Id))
                .ToListAsync();

            if (unusedTags.Any())
            {
                _context.Tags.RemoveRange(unusedTags);
                await _context.SaveChangesAsync();
            }
        }
    }
}
