using System.ComponentModel.DataAnnotations;
using warehouseapp.Enums;

namespace warehouseapp.ViewModels.Product
{
    public class ProductRequestViewModel
    {
        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50, ErrorMessage = "SKU must be at most 50 characters.")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be at most 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description must be at most 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Unit of measurement is required.")]
        public UnitOfMeasurementEnum? UnitOfMeasurement { get; set; }

        public bool HasExpiration { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [Range(0.01, 9999999, ErrorMessage = "Unit cost price must be greater than 0.")]
        public decimal UnitCostPrice { get; set; }

        [Range(0.01, 9999999, ErrorMessage = "Unit selling price must be greater than 0.")]
        public decimal UnitSellingPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Reorder level must be 0 or greater.")]
        public int ReorderLevel { get; set; } = 5;
    }
}