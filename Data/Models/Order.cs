using warehouseapp.Enums;

namespace warehouseapp.Data.Models
{
    public class Order : BaseModel
    {
        public int PartnerId{ get; set; }
        public required Partner Partner { get; set; }
        public DateTime ExpirationDate { get; set; }
        public OrderStatusEnum Status { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}