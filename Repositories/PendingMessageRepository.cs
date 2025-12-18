using YusurIntegration.Models;
using YusurIntegration.Data;
using Microsoft.EntityFrameworkCore;
namespace YusurIntegration.Repositories
{
    public class PendingMessageRepository : IPendingMessageRepository
    {
        private readonly AppDbContext _db;

        public PendingMessageRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(PendingMessage msg)
        {
            _db.PendingMessages.Add(msg);
            await _db.SaveChangesAsync();
        }

        public Task<List<PendingMessage>> GetPendingForBranch(string branchLicense)
        {
            return _db.PendingMessages
                .Where(x => x.BranchLicense == branchLicense && !x.isdelivered)
                .ToListAsync();
        }

        public async Task MarkDelivered(long id)
        {
            var msg = await _db.PendingMessages.FindAsync(id);
            if (msg != null)
            {
                msg.isdelivered = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteDelivered()
        {
            var items = _db.PendingMessages.Where(x => x.isdelivered);
            _db.PendingMessages.RemoveRange(items);
            await _db.SaveChangesAsync();
        }
    }

}
