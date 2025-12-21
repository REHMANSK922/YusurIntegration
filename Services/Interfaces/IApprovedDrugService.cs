using YusurIntegration.Models;

namespace YusurIntegration.Services.Interfaces
{
    public interface IApprovedDrugService
    {
        Task AddNewApprovalAsync(ApprovedDrug newDrug);
        Task MakeInactiveAsync(string itemno,DateTime todate);

    }
}
