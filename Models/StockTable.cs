namespace YusurIntegration.Models
{
  
    public class StockTable
    {
        public int Id { get; set; }
        public string storecode { get; set; } = string.Empty;
        public string BranchLicense { get; set; } = string.Empty;
        public string GenericCode { get; set; } = string.Empty;
        public string ItemNo { get; set; } = string.Empty;
        public int AvailableQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
