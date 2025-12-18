namespace YusurIntegration.DTOs
{
    public class StockCheckDto
    {
        public string ActivityId { get; set; } = string.Empty;
        public string GenericCode { get; set; } = string.Empty;
        public bool Available { get; set; }
        public string Reason { get; set; } = string.Empty;
        public TradeDrugDto? SelectedTradeDrug { get; set; }
        public decimal StockLevel { get; set; }
    }

    public class TradeDrugDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }


}
