using warehouseapp.Enums;

namespace warehouseapp.ViewModels.Partner
{
    public class PartnerResponseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public PartnerTypeEnum Type { get; set; }
    }
}
