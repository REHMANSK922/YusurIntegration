using YusurIntegration.Models;

namespace YusurIntegration.Repositories
{
    public interface IStockRepository
    {
        Task UpdateStockAsync(StockTable item);
        Task<int> GetStockAsync(string itemNo, string branch);
    }
}
