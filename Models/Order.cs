using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace YusurIntegration.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string VendorId { get; set; }
        public string BranchLicense { get; set; }
        public string ErxReference { get; set; }
        public string PatientNationalId { get; set; }
        public bool IsPickup { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Activity> Activities { get; set; } = new();
    }

}
