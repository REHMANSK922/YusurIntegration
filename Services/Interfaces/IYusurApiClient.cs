using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Services.Interfaces
{
    public interface IYusurApiClient
    {
        Task<bool> EnsureAuthenticatedAsync();
        Task<bool> AcceptOrderAsync(OrderAcceptRequestDto request);
        Task<bool> RejectOrderAsync(OrderRejectRequestDto request);
    }
}
