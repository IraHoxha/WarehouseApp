namespace warehouseapp.ViewModels
{
    public class CategoryResponseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public List<SubCategoryViewModel> SubCategories { get; set; } = new List<SubCategoryViewModel>();
    }

    public class SubCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
