namespace warehouseapp.Data.Models
{
    public class Tag : BaseModel
    {
        public required string Key { get; set; }
        public List<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}