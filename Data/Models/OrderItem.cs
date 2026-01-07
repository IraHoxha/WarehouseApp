using warehouseapp.Data.Models;

namespace warehouseapp.Data.Models
{
    public class OrderItem : BaseModel
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitSellingPrice { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
