using YusurIntegration.Models;
using YusurIntegration.Repositories;

namespace YusurIntegration.Services
{
    public class ApprovedDrugService
    {
        private readonly IApprovedDrugRepository _repo;

        public ApprovedDrugService(IApprovedDrugRepository repo)
        {
            _repo = repo;
        }

        public async Task AddNewApprovalAsync(ApprovedDrug newDrug)
        {
            // 🔐 rule: only one active per item
            await _repo.DeactivateAllAsync(newDrug.ItemNo);

            newDrug.IsActive = true;
            await _repo.AddAsync(newDrug);
        }

        public async Task MakeInactiveAsync(string itemno)
        {
            var drug = await _repo.GetActiveAsyncById(itemno);
            if (drug == null) return;
            drug.IsActive = false;
            await _repo.AddAsync(drug);
        }
    }

}
