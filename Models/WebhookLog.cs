using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class WebhookLog
    {
        [Key]
        public int Id { get; set; }
        public string WebhookType { get; set; }
        public string Payload { get; set; }
        public string  BranchLicense { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    }
}
