namespace YusurIntegration.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string[] Errors { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
