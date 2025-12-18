namespace YusurIntegration.Models
{
    public class Products
    {
        public int Id { get; set; }
        public required string Itemno { get; set; }
        public required  string Barcode { get; set; }
        public required string ProductName { get; set; }
        public  string? Unit { get; set; }


    }
}
