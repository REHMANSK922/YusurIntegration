using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Models
{
    public class Order
    {
        [Key]
        public required string OrderId { get; set; }

        [Required]
        public string VendorId { get; set; }
        [Required]
        public string BranchLicense { get; set; }
        [Required]
        public string ErxReference { get; set; }


        public bool IsPickup { get; set; }
        public string? Status { get; set; }

        public string? failureReason { get; set; }

        public string? DeliveryTimeSlotId { get; set; } // <-- Change 'deliveryTimeSlotId' to 'DeliveryTimeSlotId'
        public string? DeliveryTimeSlotStartTime { get; set; } // <-- Cha
                                                               // nge 'deliveryTimeSlotStartTime' to 'DeliveryTimeSlotStartTime'
        public string? DeliveryTimeSlotEndTime { get; set; } // <-- Change 'deliveryTimeSlotEndTime' to 'DeliveryTimeSlotEndTime'
        public DateOnly? DeliveryDate { get; set; } // <-- Change 'deliveryDate' to 'DeliveryDate'
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Patient? Patient { get; set; } // <-- Change 'patient' to 'Patient'
        public List<Activity> Activities { get; set; } = new();
        public ShippingAddress? ShippingAddress { get; set; } // <-- Change 'shippingAddress' to 'ShippingAddress'


    }

}
