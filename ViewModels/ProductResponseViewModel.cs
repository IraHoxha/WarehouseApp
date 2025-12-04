namespace warehouseapp.ViewModels
{
    public class ProductResponseViewModel
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal QuantityInStock { get; set; }
    }
}
