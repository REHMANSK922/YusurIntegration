namespace YusurIntegration.Models
{
    public class PendingMessage
    {
        public int Id { get; set; }
        public string MessageId { get; set; } = System.Guid.NewGuid().ToString();
        public string BranchLicense { get; set; } = "";
        public string MessageType { get; set; } = "";
        public string PayloadJson { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool isdelivered { get; set; } = false;
    }
}
