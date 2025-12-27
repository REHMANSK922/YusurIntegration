using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YusurIntegration.Models
{
    public class WebhookLog
    {
        [Key]
        public int Id { get; set; }
        public string? OrderId { get; set; }
        public string? WebhookType { get; set; }

        [Column(TypeName = "BLOB SUB_TYPE TEXT")]
        public string? Payload { get; set; }
        public string?  BranchLicense { get; set; }
        public string? Status { get; set; }

        public string? BranchConnected { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    }
}
