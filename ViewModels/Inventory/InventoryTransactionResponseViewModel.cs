using warehouseapp.Enums;

namespace warehouseapp.ViewModels.Inventory
{
    public class InventoryTransactionResponseViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }
        public InventoryTransactionSourceEnum Source { get; set; }
        public ReturnReasonEnum? ReturnReason { get; set; }
        
        public int Quantity { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public string? BatchNumber { get; set; }
        public decimal? UnitPrice { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? CompletedAt { get; set; } 
    }

}
