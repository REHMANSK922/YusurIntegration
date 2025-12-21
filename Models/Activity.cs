using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class Activity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int OrderId { get; set; }

        public Order Order { get; set; }
        public string ActivityIdFromYusur { get; set; }
        public string GenericCode { get; set; }
        public string? Instructions { get; set; }
        public string? ArabicInstructions { get; set; }
        public string Duration { get; set; }
        public int? Refills { get; set; }
        
        public string SelectedTradeCode { get; set; }
        public int SelectedQuantity { get; set; }
        public string Itemno { get; set; }

        public List<TradeDrugs> TradeDrugs { get; set; } = new();
    }

}
