using System.ComponentModel.DataAnnotations;
using YusurIntegration.Models.Enums;

namespace YusurIntegration.Models
{
    public class OrderStatusHistory
    {
        [Key]
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string? FailureReason { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
