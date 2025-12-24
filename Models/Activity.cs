using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YusurIntegration.Models
{
    public class Activity
    {
        [Key]
        public string ActivityId { get; set; }
        [Required]
        public string OrderId { get; set; }

       
        public Order Order { get; set; }
       
        public string GenericCode { get; set; }
        public string? Instructions { get; set; }
        public string? ArabicInstructions { get; set; }
        public string Duration { get; set; }
        public int? Refills { get; set; }

        public string SelectedTradeCode { get; set; }
        public int SelectedQuantity { get; set; }
        public string authStatus { get; set; }
        public string rejectionReason { get; set; }
        public bool Isapproved { get; set; }
        public string Itemno { get; set; }


        public virtual List<TradeDrugs> TradeDrugs { get; set; } = new();
    }

}
