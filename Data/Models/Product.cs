using warehouseapp.Enums;

namespace warehouseapp.Data.Models
{
    public class Product : BaseModel
    {
        public required string SKU { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public UnitOfMeasurementEnum UnitOfMeasurement { get; set; }
        public bool HasExpiration { get; set; }

        public int CategoryId { get; set; }
        public required Category Category { get; set; }

        public decimal UnitCostPrice { get; set; }
        public decimal UnitSellingPrice { get; set; }

        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; } = 10;

        public List<ProductTag> ProductTags { get; set; } = new();
        public List<OrderItem> OrderItems { get; set; } = new();
        public List<InventoryTransaction> InventoryTransactions { get; set; } = new();
    }
}
