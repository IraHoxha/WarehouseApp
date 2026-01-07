namespace warehouseapp.ViewModels.Category
{
    public class CategorySelectItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
    }
}
