namespace warehouseapp.ViewModels.Category
{
    public class CategoryRequestViewModel
    {
        public required string Name { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
