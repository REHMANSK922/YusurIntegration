namespace YusurIntegration.Models
{
    public class ActiveMedication
    {
        public string OverallStatus { get; set; }
        public string PrescriptionDate { get; set; }
        public List<MedicationActivity> Activities { get; set; }
    }
}
