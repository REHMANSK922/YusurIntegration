namespace YusurIntegration.DTOs
{
    public class ItemValidation
    {
        public string ActivityId { get; set; } = string.Empty;
        public string GenericCode { get; set; } = string.Empty;
        public string? ItemNo { get; set; }
        public bool IsApproved { get; set; }
        public bool IsInStock { get; set; }
        public decimal RequiredQty { get; set; }
        public decimal AvailableQty { get; set; }
        public string ValidationMessage { get; set; } = string.Empty;
    }
}
