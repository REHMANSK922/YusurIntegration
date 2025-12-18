using System.Net.Http.Json;
namespace YusurIntegration.Services
{
    public class YusurApiClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly string _base;
        public YusurApiClient(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
            _base = _config.GetValue<string>("Yusur:ApiBaseUrl");
        }

        public async Task AcceptOrderAsync(string orderId, object activities)
        {
            var url = _base.TrimEnd('/') + "/order/accept";
            await _http.PostAsJsonAsync(url, new { orderId, activities });
        }

        public async Task RejectOrderAsync(string orderId, string reason)
        {
            var url = _base.TrimEnd('/') + "/order/reject";
            await _http.PostAsJsonAsync(url, new { orderId, rejectionReason = reason });
        }

        public async Task SendAwbAsync(string orderId, string awb)
        {
            var url = _base.TrimEnd('/') + "/order/awb";
            await _http.PostAsJsonAsync(url, new { orderId, awb });
        }

        public async Task ConfirmDispenseAsync(string orderId, object dto)
        {
            var url = _base.TrimEnd('/') + "/order/dispense";
            await _http.PostAsJsonAsync(url, dto);
        }
    }
}
