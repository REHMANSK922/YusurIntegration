using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Services.Interfaces
{
    public interface IAuthorizationService
    {
        //Task<bool> SubmitAuthorizationAsync(string orderId);
        //Task<bool> ResubmitAuthorizationAsync(ResubmitAuthorizationRequestDto dto);

        Task<(bool Success, string Message)> SubmitAuthorizationAsync(string orderId);
        Task<(bool Success, string Message)> ResubmitAuthorizationAsync(ResubmitAuthorizationRequestDto dto);
    }
}
