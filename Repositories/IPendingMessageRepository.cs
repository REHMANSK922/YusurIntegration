using YusurIntegration.Models;

namespace YusurIntegration.Repositories
{
    public interface IPendingMessageRepository
    {
        Task AddAsync(PendingMessage msg);
        Task<List<PendingMessage>> GetPendingForBranch(string branchLicense);
        Task MarkDelivered(long id);
        Task DeleteDelivered();
    }

}
