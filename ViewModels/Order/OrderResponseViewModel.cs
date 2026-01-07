using warehouseapp.Enums;

namespace warehouseapp.ViewModels.Order
{
    public class OrderResponseViewModel
    {
        public int Id { get; set; }

        public int PartnerId { get; set; }           
        public string PartnerName { get; set; } = string.Empty;
        public OrderTypeEnum Type { get; set; } 
        public DateTime ExpirationDate { get; set; }
        public OrderStatusEnum Status { get; set; }
        public List<OrderItemResponseViewModel> Items { get; set; } = new();
         public DateTime? CompletedAt { get; set; } 
    }
}
