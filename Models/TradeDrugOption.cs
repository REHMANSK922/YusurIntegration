using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class TradeDrugs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ActivityId { get; set; }
        public Activity Activity { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
