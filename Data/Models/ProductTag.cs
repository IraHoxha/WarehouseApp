namespace warehouseapp.Data.Models
{
    public class ProductTag : BaseModel
    {
        public int ProductId { get; set; }
        public required Product Product { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public required string Value { get; set; }
    }
}