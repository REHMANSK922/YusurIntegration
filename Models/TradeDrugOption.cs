using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YusurIntegration.Models
{
    public class TradeDrugs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ActivityId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public virtual Activity Activity { get; set; }
    }
}
