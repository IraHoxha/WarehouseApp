namespace warehouseapp.Data.Models
{
    public class Category: BaseModel
    {
        public required string Name{ get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public List<Category> SubCategories { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}