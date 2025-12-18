namespace YusurIntegration.DTOs
{
    public class OrderValidationResult
    {
        public bool IsValid => AllItemsApproved && AllItemsInStock;
        public bool AllItemsApproved { get; set; }
        public bool AllItemsInStock { get; set; }
        public List<ItemValidation> ItemValidations { get; set; } = new();
        public string? RejectionReason { get; set; }
    }
}
