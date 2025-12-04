namespace warehouseapp.Data.Models
{
    public class OrderItem : BaseModel
    {
        public int OrderId { get; set; }
        public required Order Order { get; set; }
        public int ProductId { get; set; }
        public required Product Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitSellingPrice { get; set; }
    }
}