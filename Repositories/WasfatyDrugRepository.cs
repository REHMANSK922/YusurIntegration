using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;

namespace YusurIntegration.Repositories
{
    public class WasfatyDrugRepository : IWasfatyDrugRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<WasfatyDrugRepository> _logger;


        public WasfatyDrugRepository(
            AppDbContext db,
            ILogger<WasfatyDrugRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<List<WasfatyDrugs>> GetAllActiveAsync()
        {
            return await _db.WasfatyDrugs
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<WasfatyDrugs?> GetByGenericCodeAsync(string genericCode)
        {
            throw new NotImplementedException();
        }
        public async Task<string?> GetSfdaCodeAsync(string genericCode)
        {
            return await _db.WasfatyDrugs
                .Where(x => x.GenericCode == genericCode)
                .Select(x => x.DrugCode)
                .FirstOrDefaultAsync();
        }

        public async Task<WasfatyDrugs?> GetByDrugCodeAsync(string drugcode)
        {
            return await _db.WasfatyDrugs.FirstOrDefaultAsync(a => a.DrugCode == drugcode);
        }


    }
}
