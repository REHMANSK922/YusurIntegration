using YusurIntegration.Models;

namespace YusurIntegration.Services.Interfaces
{
    public interface IDatabaseService
    {
        Task<IEnumerable<Order>> GetOrdersByLicenseAsync(string branchLicense, string? status = null);
        Task<IEnumerable<Activity>> GetActivitiesByLicenseAsync(string branchLicense, string? status = null);
        Task<IEnumerable<TradeDrug>> GetTradeDrugsByLicenseAsync(string branchLicense, string? status = null);
        Task<object> GetAllDataByLicenseAsync(string branchLicense, string? status = null);
        Task<int> DeleteOrdersByLicenseAsync(string branchLicense, string? status = null);
        Task<int> DeleteActivitiesByLicenseAsync(string branchLicense, string? status = null);
        Task<int> DeleteTradeDrugsByLicenseAsync(string branchLicense, string? status = null);
        Task<int> DeleteAllDataByLicenseAsync(string branchLicense, string? status = null);
        Task<int> GetRecordCountsByLicenseAsync(string branchLicense, string? status = null);
    }
}
