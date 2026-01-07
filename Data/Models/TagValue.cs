namespace warehouseapp.Data.Models
{
    public class TagValue : BaseModel
    {
        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
