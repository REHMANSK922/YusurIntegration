using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.DTOs
{
    public class DispenseShippingAddressDto
    {
        public string phone { get; set; }
        public string name { get; set; } 
        public string cityId { get; set; }
        public string area { get; set; }
        public string buildingNumber { get; set; }
        public string streetAddress1 { get; set; }
        public string streetAddress2 { get; set; }
        public CoordinatesDto coordinates { get; set; }
    }
}
