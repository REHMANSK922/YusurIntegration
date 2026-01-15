namespace YusurIntegration.Models
{
    public class ActiveMedicationResponse
    {
        public List<ActiveMedication> ActiveMedications { get; set; }
        public List<ApiError> Errors { get; set; }
    }
}
