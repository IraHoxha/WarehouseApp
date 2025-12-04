using warehouseapp.Enums;

namespace warehouseapp.Data.Models
{
    public class Partner : BaseModel
    {
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public PartnerTypeEnum Type { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}