namespace YusurIntegration.DTOs
{
    public class OrderStockCheckResult
    {
        public string OrderId { get; set; } = string.Empty;
        public string BranchLicense { get; set; } = string.Empty;
        public List<StockCheckDto> ActivityChecks { get; set; } = new();
        public bool AllItemsAvailable => ActivityChecks.All(a => a.Available);
        public int TotalItems => ActivityChecks.Count;
        public int AvailableItems => ActivityChecks.Count(a => a.Available);
        public int UnavailableItems => ActivityChecks.Count(a => !a.Available);
    }
}
