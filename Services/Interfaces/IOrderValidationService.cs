using YusurIntegration.DTOs;
using YusurIntegration.Models;

namespace YusurIntegration.Services.Interfaces
{
    public interface IOrderValidationService
    {
        Task<ActivityValidationResultDto> ValidateActivityAsync(
            string branchLicense,
            DateTime orderDate,
            List<TradeDrugs> tradeDrugs);
    }
}
