using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.DTOs
{
    public class PrescriptionDispenseRequestDto
    {
        // These will be sent as Query Parameters
        public string PatientNationalId { get; set; } = string.Empty;
        public string PrescriptionReferenceNumber { get; set; } = string.Empty;
        public bool IsPickup { get; set; }
        public string? DeliveryPeriodId { get; set; }
        public string? DeliveryDate { get; set; } // YYYY-MM-DD

        // This will be sent as the JSON Body
        public DispenseShippingAddressDto ShippingAddress { get; set; } = new();
    }
}
