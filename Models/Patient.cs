using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class Patient
    {
        [Key] 
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string nationalId { get; set; }
        public string memberId { get; set; }
        public string firstName { get; set; }
        public string? lastName { get; set; }
        public string? bloodGroup { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
    }
}
