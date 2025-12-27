using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;
using YusurIntegration.Services.Interfaces;

namespace YusurIntegration.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IOrderRepository orderRepository, ILogger<DatabaseService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public Task<IEnumerable<Order>> GetOrdersByLicenseAsync(string branchLicense, string? status = null)
        {
            return _orderRepository.GetOrdersByLicenseAsync(branchLicense, status);
        }

        public Task<IEnumerable<Activity>> GetActivitiesByLicenseAsync(string branchLicense, string? status = null)
        {
            return _orderRepository.GetActivitiesByLicenseAsync(branchLicense, status);
        }

        public Task<IEnumerable<TradeDrug>> GetTradeDrugsByLicenseAsync(string branchLicense, string? status = null)
        {
            return _orderRepository.GetTradeDrugsByLicenseAsync(branchLicense, status);
        }

        public Task<object> GetAllDataByLicenseAsync(string branchLicense, string? status = null)
        {
            return _orderRepository.GetAllDataByLicenseAsync(branchLicense, status);
        }

        public Task<int> DeleteOrdersByLicenseAsync(string branchLicense, string? status = null)
        {
            return _orderRepository.DeleteOrdersByLicenseAsync(branchLicense, status);
        }

        public Task<int> DeleteActivitiesByLicenseAsync(string branchLicense, string? status = null)
        {
            return _orderRepository.DeleteActivitiesByLicenseAsync(branchLicense, status);
        }

        public Task<int> DeleteTradeDrugsByLicenseAsync(string branchLicense, string? status = null)
        {
            return _orderRepository.DeleteTradeDrugsByLicenseAsync(branchLicense, status);
        }

        public Task<int> DeleteAllDataByLicenseAsync(string branchLicense, string? status = null)
        {
            return _orderRepository.DeleteAllDataByLicenseAsync(branchLicense, status);
        }

        public Task<int> GetRecordCountsByLicenseAsync(string branchLicense, string? status = null)
        {
            return _orderRepository.GetRecordCountsByLicenseAsync(branchLicense, status);
        }
    }
}
