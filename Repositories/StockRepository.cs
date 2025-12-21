using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;

namespace YusurIntegration.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly StockDbContext _db;

        public StockRepository(StockDbContext db)
        {
            _db = db;
        }

        public async Task UpdateStockAsync(StockTable item)
        {
            var exist = await _db.StockTable
                .FirstOrDefaultAsync(x => x.ItemNo == item.ItemNo && x.BranchLicense == item.BranchLicense);

            if (exist == null)
                _db.StockTable.Add(item);
            else
            {
                exist.AvailableQuantity = item.AvailableQuantity;
                exist.LastUpdated = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        public Task<int> GetStockAsync(string generic, string branch)
        {
            return _db.StockTable
                .Where(x => x.GenericCode == generic && x.BranchLicense == branch)
                .Select(x => x.AvailableQuantity)
                .FirstOrDefaultAsync();
        }
         


    }

}
