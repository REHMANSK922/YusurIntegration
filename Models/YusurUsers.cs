using System.ComponentModel.DataAnnotations;

namespace YusurIntegration.Models
{
    public class YusurUsers
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string? Username { get; set; }
        [StringLength(100)]
        public string? Password { get; set; }
        [StringLength(100)]
        public string? Licence { get; set; }
        [StringLength(50)]
        public string? storename { get; set; }
        [StringLength(10)]
        public string? storecode { get; set; }

    }
}
