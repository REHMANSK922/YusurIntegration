using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YusurIntegration.Models
{
    public class Activity
    {
        [Key]
        public string ActivityId { get; set; }

        [Required]
        [ForeignKey(nameof(Order))]
        public string OrderId { get; set; }

        [Required]
        public string GenericCode { get; set; }
        public string? Instructions { get; set; }
        public string? ArabicInstructions { get; set; }

        [Required]
        public string Duration { get; set; }
        public int? Refills { get; set; }

        public string? SelectedTradeCode { get; set; }
        public int? SelectedQuantity { get; set; }
        public string? authStatus { get; set; }
        public string? rejectionReason { get; set; }
        public bool? Isapproved { get; set; }
        public string? Itemno { get; set; }

        public Order Order { get; set; }
        public virtual List<TradeDrug> TradeDrugs { get; set; } = new();
    }

}
