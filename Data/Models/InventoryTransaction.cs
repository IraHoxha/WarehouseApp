using warehouseapp.Enums;

namespace warehouseapp.Data.Models
{
    public class InventoryTransaction : BaseModel
    {
        public TransactionTypeEnum TransactionType { get; set; }
        public decimal Quantity{ get; set; }
        public string? BatchNumber { get; set; } 
        public int ProductId { get; set; }
        public required Product Product { get; set; }
        public int PartnerId { get; set; }
        public required Partner Partner { get; set; }
        public int OrderId { get; set; }
        public required Order Order { get; set; }
        public decimal? UnitPrice { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}