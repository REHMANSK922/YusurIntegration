using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YusurIntegration.Models
{
    public class ShippingAddress
    {
        [Key] // Sets OrderId as the Primary Key
        [ForeignKey(nameof(Order))]
        public string OrderId { get; set; }
        public  string? addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string? area { get; set; }
        public string? city { get; set; }
        public  Coordinates? Coordinates { get; set; }
        public virtual Order Order { get; set; }
    }
}
