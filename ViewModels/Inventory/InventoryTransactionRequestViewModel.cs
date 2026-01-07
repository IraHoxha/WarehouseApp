using System.ComponentModel.DataAnnotations;
using warehouseapp.Enums;

namespace warehouseapp.ViewModels.Inventory
{
    public class InventoryTransactionRequestViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int PartnerId { get; set; }

        public int? OrderId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        [Required]
        public TransactionTypeEnum TransactionType { get; set; }

        [Required]
        public InventoryTransactionSourceEnum Source { get; set; }

        public ReturnReasonEnum? ReturnReason { get; set; } 

        public DateTime? ExpirationDate { get; set; }
        public string? BatchNumber { get; set; }
    }
}
