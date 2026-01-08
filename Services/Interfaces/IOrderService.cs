using YusurIntegration.DTOs;
using YusurIntegration.Models;
 

namespace YusurIntegration.Services.Interfaces
{
    public interface IOrderService
    {

        Task HandleNewOrderAsync(YusurPayloads.NewOrderDto dto);
        Task HandleOrderAllocationAsync(YusurPayloads.OrderAllocationDto dto);
        Task<(bool Success, string? ErrorMessage, Order? Data)> HandleAuthorizationResponseAsync(YusurPayloads.AuthorizationResponseDto dto);
        Task HandleStatusUpdateAsync(YusurPayloads.StatusUpdateDto dto);

        Task HandleSendYusurOrderAccept(YusurPayloads.OrderAcceptRequestDto dto,string branchno);
    }
}
