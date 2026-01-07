using warehouseapp.Enums;

namespace warehouseapp.Data.Models
{
    public class InventoryTransaction : BaseModel
    {
        public int ProductId { get; set; }
        public required Product Product { get; set; }

        public int PartnerId { get; set; }
        public required Partner Partner { get; set; }

        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }
        public InventoryTransactionSourceEnum Source { get; set; }
        public string? ReturnReason { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public string? BatchNumber { get; set; }

        public DateTime? CompletedAt { get; set; } 
    }

}