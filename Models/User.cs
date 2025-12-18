using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class User
    {
        [Key] 
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = ""; // store hashed
        public string Role { get; set; } = "admin";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
