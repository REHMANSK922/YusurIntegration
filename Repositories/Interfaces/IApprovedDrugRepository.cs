using YusurIntegration.Models;

namespace YusurIntegration.Repositories.Interfaces
{
    public interface IApprovedDrugRepository
    {
 

        Task<ApprovedDrug?> GetActiveAsync(string itemNo, DateTime orderDate); // get active by item no and date
        Task<List<ApprovedDrug?>> GetByItemNoAsync(string itemNo);
        Task AddAsync(ApprovedDrug drug);
        Task DeactivateAllAsync(string itemNo);
        Task<ApprovedDrug?> GetActiveAsyncById(string itemNo);
        Task<ApprovedDrug?> GetValidAsyncBySfda(
        string sfdacode,
        DateTime orderDate);
        Task<bool> DeleteItemnoById(string itemno, DateTime frmdt, DateTime todt);

        //Task<ApprovedDrug?> GetBestApprovedDrugAsync(
        //                    string genericCode,
        //                    List<string> tradeDrugCodes,
        //                    DateTime orderDate,
        //                    string branchLicense,
        //                    int requiredQty);

    }
}
 
