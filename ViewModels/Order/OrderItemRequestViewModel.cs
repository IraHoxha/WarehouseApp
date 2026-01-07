using System.ComponentModel.DataAnnotations;

public class OrderItemRequestViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
    public decimal UnitSellingPrice { get; set; }

    public string? BatchNumber { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
