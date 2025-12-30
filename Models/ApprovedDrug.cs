using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{

    public class ApprovedDrug
    {
        public int Id { get; set; }

        [Required]
        public string ItemNo { get; set; }          // link to stock
        [Required]
        public string Sfdacode { get; set; }     // link to Wasfaty

        public string? GenericCode { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PriorityOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } 
    }

}
