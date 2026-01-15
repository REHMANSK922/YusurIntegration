namespace YusurIntegration.Models
{
    public class MedicationActivity
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public double Quantity { get; set; }
        public int Duration { get; set; }
        public string UnitId { get; set; }
        public int Refills { get; set; }
        public string Instructions { get; set; }
        public string ArabicInstructions { get; set; }
        public string Start { get; set; }
        public List<Observation> Observation { get; set; }
    }
}
