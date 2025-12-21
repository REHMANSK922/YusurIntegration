using YusurIntegration.DTOs;
using YusurIntegration.Models;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Services.Interfaces
{
    public interface IOrderService
    {

        Task HandleNewOrderAsync(NewOrderDto dto);
        Task HandleOrderAllocationAsync(OrderAllocationDto dto);
        Task HandleAuthorizationResponseAsync(AuthorizationResponseDto dto);
        Task HandleStatusUpdateAsync(StatusUpdateDto dto);        
    }
}
