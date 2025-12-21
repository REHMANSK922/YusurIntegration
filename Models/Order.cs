using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using static YusurIntegration.DTOs.YusurPayloads;

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
        public Patient? Patient { get; set; } // <-- Change 'patient' to 'Patient'
        public List<Activity> Activities { get; set; } = new();
        public ShippingAddress? ShippingAddress { get; set; } // <-- Change 'shippingAddress' to 'ShippingAddress'
            
  
        public bool IsPickup { get; set; }
        public string Status { get; set; }
        public string? DeliveryTimeSlotId { get; set; } // <-- Change 'deliveryTimeSlotId' to 'DeliveryTimeSlotId'
        public TimeOnly? DeliveryTimeSlotStartTime { get; set; } // <-- Change 'deliveryTimeSlotStartTime' to 'DeliveryTimeSlotStartTime'
        public TimeOnly? DeliveryTimeSlotEndTime { get; set; } // <-- Change 'deliveryTimeSlotEndTime' to 'DeliveryTimeSlotEndTime'
        public DateOnly? DeliveryDate { get; set; } // <-- Change 'deliveryDate' to 'DeliveryDate'
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
    }

}
