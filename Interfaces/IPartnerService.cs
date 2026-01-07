using warehouseapp.ViewModels.Partner;

namespace warehouseapp.Interfaces
{
    public interface IPartnerService
    {
        Task<List<PartnerResponseViewModel>> GetAllAsync();
        Task<PartnerResponseViewModel> GetByIdAsync(int id);
        Task<List<PartnerResponseViewModel>> FilterAsync(string? search, int? type);
        Task<PartnerResponseViewModel> CreateAsync(PartnerRequestViewModel model);
        Task<PartnerResponseViewModel> UpdateAsync(int id, PartnerRequestViewModel model);
        Task DeleteAsync(int id);
    }

}
