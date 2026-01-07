using AutoMapper;
using Microsoft.EntityFrameworkCore;
using warehouse.Data;
using warehouseapp.Data.Models;
using warehouseapp.Exceptions;
using warehouseapp.Interfaces;
using warehouseapp.ViewModels.Partner;

namespace warehouseapp.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public PartnerService(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PartnerResponseViewModel> CreateAsync(PartnerRequestViewModel model)
        {
            Normalize(model);

            ValidateRequiredFields(model);

            await EnsureUniquenessAsync(model);

            var partner = _mapper.Map<Partner>(model);

            _context.Partners.Add(partner);
            await _context.SaveChangesAsync();

            return _mapper.Map<PartnerResponseViewModel>(partner);
        }

        public async Task<PartnerResponseViewModel> UpdateAsync(int id, PartnerRequestViewModel model)
        {
            var partner = await _context.Partners.FindAsync(id)
                ?? throw new NotFoundException("Partner not found.");

            Normalize(model);

            ValidateRequiredFields(model);

            await EnsureUniquenessAsync(model, id);

            var originalType = partner.Type;

            _mapper.Map(model, partner);

            partner.Type = originalType;
            partner.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<PartnerResponseViewModel>(partner);
        }
    

        public async Task DeleteAsync(int id)
        {
            var partner = await _context.Partners
                .Include(p => p.Orders)
                .Include(p => p.InventoryTransactions)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException("Partner not found.");

            if (partner.Orders.Any())
                throw new ValidationException("Cannot delete partner with existing orders.");

            if (partner.InventoryTransactions.Any())
                throw new ValidationException("Cannot delete partner with inventory history.");

            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();
        }

        public async Task<PartnerResponseViewModel> GetByIdAsync(int id)
        {
            var partner = await _context.Partners
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException("Partner not found.");

            return _mapper.Map<PartnerResponseViewModel>(partner);
        }

        public async Task<List<PartnerResponseViewModel>> GetAllAsync()
        {
            var partners = await _context.Partners
                .OrderBy(p => p.Name)
                .ToListAsync();

            return _mapper.Map<List<PartnerResponseViewModel>>(partners);
        }

        public async Task<List<PartnerResponseViewModel>> FilterAsync(string? search, int? type)
        {
            IQueryable<Partner> query = _context.Partners;

            if (type.HasValue)
                query = query.Where(p => (int)p.Type == type.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    (p.Email != null && p.Email.Contains(search)) ||
                    (p.PhoneNumber != null && p.PhoneNumber.Contains(search)));
            }

            var list = await query
                .OrderBy(p => p.Name)
                .ToListAsync();

            return _mapper.Map<List<PartnerResponseViewModel>>(list);
        }
    
    
        private static void Normalize(PartnerRequestViewModel model)
        {
            model.Name = model.Name?.Trim();
            model.Email = model.Email?.Trim();
            model.PhoneNumber = model.PhoneNumber?.Trim();
            model.Address = model.Address?.Trim();
        }

        private static void ValidateRequiredFields(PartnerRequestViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ValidationException("Name is required.");

            if (model.Name.Length < 2)
                throw new ValidationException("Name must be at least 2 characters.");

            if (model.Type == null)
                throw new ValidationException("Partner type is required.");

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                throw new ValidationException("Phone number is required.");
        }

        private async Task EnsureUniquenessAsync(PartnerRequestViewModel model, int? excludeId = null)
        {
            var query = _context.Partners.AsQueryable();

            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await query.AnyAsync(p =>
                    p.PhoneNumber != null &&
                    p.PhoneNumber == model.PhoneNumber))
            {
                throw new ValidationException("Phone number is already in use.");
            }

            if (!string.IsNullOrWhiteSpace(model.Email) &&
                await query.AnyAsync(p =>
                    p.Email != null &&
                    p.Email.ToLower() == model.Email.ToLower()))
            {
                throw new ValidationException("Email is already in use.");
            }
        }

    }
}
