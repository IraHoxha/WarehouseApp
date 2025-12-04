using warehouseapp.Enums;

namespace warehouseapp.Data.Models
{
    public class Product : BaseModel
    {
        public required string SKU { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }        
        public UnitOfMeasurementEnum UnitOfMeasurement { get; set; }    
        public bool HasExpiration { get; set; }
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
        public decimal UnitCostPrice { get; set; }
        public decimal UnitSellingPrice { get; set; }
        public decimal QuantityInStock { get; set; }
        public List<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();



        

    }
}