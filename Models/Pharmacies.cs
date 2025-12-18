using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class Pharmacies
    {
        [Key]
        public int Id { get; set; }
        public string PharmacyName { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string GroupId { get; set; }
        public bool isGroup { get; set; }
        public string YusurUser { get; set; }
        public string YusurPassword { get; set; }
        public string Code { get; set; }


    }
}
