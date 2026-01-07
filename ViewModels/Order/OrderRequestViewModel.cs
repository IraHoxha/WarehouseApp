using System.ComponentModel.DataAnnotations;
using warehouseapp.Enums;

public class OrderRequestViewModel
{
    [Required]
    public OrderTypeEnum Type { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a partner.")]
    public int PartnerId { get; set; }

    [Required(ErrorMessage = "Order expiration date is required.")]
    public DateTime ExpirationDate { get; set; }
    public List<OrderItemRequestViewModel> Items { get; set; } = new();
}
