using warehouseapp.ViewModels.Tag;
namespace warehouseapp.ViewModels.Product
{
    public class ProductResponseViewModel
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal UnitCostPrice { get; set; }
        public decimal UnitSellingPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }

        public List<ProductTagDisplayViewModel> Tags { get; set; } = new();
    }
}
