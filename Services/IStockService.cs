using YusurIntegration.DTOs;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Services
{
    public interface IStockService
    {
        Task<StockCheckDto> CheckActivityStockAsync(
            string branchLicense,
            ActivityDto activity);

        // Check multiple activities (entire order)
        Task<OrderStockCheckResult> CheckOrderStockAsync(
            string branchLicense,
            string orderId,
            List<ActivityDto> activities);

        // Reserve stock for accepted order
        Task<bool> ReserveOrderStockAsync(string branchLicense, List<StockCheckDto> activities);

        // Release stock for rejected/cancelled order
        Task<bool> ReleaseOrderStockAsync(string branchLicense, List<StockCheckDto> activities);
    }
}
