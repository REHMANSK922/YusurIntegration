using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Services.Interfaces
{
    public interface IYusurApiClient
    {
        Task<bool> EnsureAuthenticatedAsync();
        Task<ApiErrorResponseDto?> AcceptOrderAsync(OrderAcceptRequestDto request);
        Task<ApiErrorResponseDto?> RejectOrderAsync(OrderRejectRequestDto request);
    }
}
