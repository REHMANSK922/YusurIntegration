using YusurIntegration.DTOs;
using YusurIntegration.Models;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Services.Interfaces
{
    public interface IYusurApiClient
    {
        Task<bool> EnsureAuthenticatedAsync();
        Task<ApiErrorResponseDto?> AcceptOrderAsync(OrderAcceptRequestDto request);
        Task<ApiErrorResponseDto?> RejectOrderAsync(OrderRejectRequestDto request);
        Task<ApiErrorResponseDto?> CancelOrderAsync(OrderCancelRequestDto request);
        Task<ApiErrorResponseDto?> ConfirmOrderPickupAsync(OrderConfirmPickup request);

        Task<DispenseSuccessResponse?> PrescriptionDispenseAsync(PrescriptionDispenseRequestDto request);
        Task<List<ActiveMedicationResponse>?> GetActiveMedicationsAsync(string patientId);
        
        Task<ApiErrorResponseDto?> SubmitAuthorization(SubmitAuthorizationRequestDto request);
        Task<ApiErrorResponseDto?> ReSubmitAuthorization(ResubmitAuthorizationRequestDto request);

        Task<List<DeliveryPeriod>> GetDeliveryPeriodsAsync();
        Task<List<ResponseRejectReason>> GetRejectionReasons();
        Task<List<City>> GetCities(string city);


    }
}
