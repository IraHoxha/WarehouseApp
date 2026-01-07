namespace warehouseapp.Data.Models
{
    public class Tag : BaseModel
    {
        public required string Key { get; set; }

        public ICollection<TagValue> Values { get; set; } = new List<TagValue>();
    }

}