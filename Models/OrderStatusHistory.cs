using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class OrderStatusHistory
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
