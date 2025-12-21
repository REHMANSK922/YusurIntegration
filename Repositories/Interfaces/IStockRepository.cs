using YusurIntegration.Models;

namespace YusurIntegration.Repositories.Interfaces
{
    public interface IStockRepository
    {
        Task UpdateStockAsync(StockTable item);
        Task<int> GetStockAsync(string itemNo, string branch);
    }
}
