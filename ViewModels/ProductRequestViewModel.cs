using warehouseapp.Enums;

namespace warehouseapp.ViewModels
{
    public class ProductRequestViewModel
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public UnitOfMeasurementEnum UnitOfMeasurement { get; set; }
        public bool HasExpiration { get; set; }
        public int CategoryId { get; set; }
        public decimal UnitCostPrice { get; set; }
        public decimal UnitSellingPrice { get; set; }
    }
}
