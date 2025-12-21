namespace YusurIntegration.Models
{
    public class ShippingAddress
    {
        public string OrderId { get; set; }
        public  string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string area { get; set; }
        public string city { get; set; }
        public  Coordinates Coordinates { get; set; }
    }
}
