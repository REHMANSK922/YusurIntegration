using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YusurIntegration.Models
{
    public class TradeDrug
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [ForeignKey(nameof(Activity))]
        public string ActivityId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Quantity { get; set; }
        public virtual Activity Activity { get; set; }
    }
}
