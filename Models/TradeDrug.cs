using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YusurIntegration.Models
{
    public class TradeDrug
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Activity))]
        public int ActivityForeignId { get; set; }

        [Required]
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
