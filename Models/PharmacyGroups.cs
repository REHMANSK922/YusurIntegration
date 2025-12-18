using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class PharmacyGroups
    {
        [Key]
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
    }
}
