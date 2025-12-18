using YusurIntegration.Models;

namespace YusurIntegration.Repositories
{
    public interface IWasfatyDrugRepository
    {
        Task<WasfatyDrugs?> GetByGenericCodeAsync(string genericCode);
        Task<string?> GetSfdaCodeAsync(string genericCode);
        Task<List<WasfatyDrugs>> GetAllActiveAsync();
        Task<WasfatyDrugs?> GetByDrugCodeAsync(string drugCode);

    }
}
