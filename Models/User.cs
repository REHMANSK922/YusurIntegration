using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class User
    {
        [Key] 
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = "";
        [Required]
        public string PasswordHash { get; set; } = ""; // store hashed


        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "admin"; // "admin", "dbadmin", "viewer"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Optional: Add refresh token for token refresh
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

    }
}
