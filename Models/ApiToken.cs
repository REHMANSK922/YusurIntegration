using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class ApiToken
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string TokenType { get; set; } = "YUSUR"; // For future: can support multiple APIs
        [StringLength(2000)]
        public string? AccessToken { get; set; }
        [StringLength(100)]
        public string? Username { get; set; }
        [StringLength(100)]
        public string? Licence { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsValid { get; set; }
        public DateTime? LastUsed { get; set; }
    }
}
