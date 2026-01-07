using System.ComponentModel.DataAnnotations;
using warehouseapp.Enums;

public class OrderReturnRequestViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Return quantity must be greater than zero.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Please select a return reason.")]
    public ReturnReasonEnum? ReturnReason { get; set; }
}
