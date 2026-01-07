using warehouseapp.Enums;

namespace warehouseapp.ViewModels.Product
{
    public class ProductEditResponseViewModel
    {
        public int Id { get; set; }

        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public UnitOfMeasurementEnum UnitOfMeasurement { get; set; }

        public int CategoryId { get; set; }

        public decimal UnitCostPrice { get; set; }

        public decimal UnitSellingPrice { get; set; }

        public bool HasExpiration { get; set; }


        public int ReorderLevel { get; set; }
    }
}
