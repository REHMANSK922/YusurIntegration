using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class TradeDrugs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ActivityIdFromYusur { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
