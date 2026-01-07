using System.ComponentModel.DataAnnotations;

namespace warehouseapp.ViewModels.Category
{
    public class CategoryRequestViewModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
