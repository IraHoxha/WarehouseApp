namespace warehouseapp.ViewModels.Order
{
    public class OrderItemResponseViewModel
    {
        public int ProductId { get; set; }         
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal UnitSellingPrice { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
