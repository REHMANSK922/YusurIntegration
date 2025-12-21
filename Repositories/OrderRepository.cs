using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;

namespace YusurIntegration.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;

        public OrderRepository(AppDbContext db)
        {
            _db = db;
        }
        public Task<Order?> GetByOrderId(string orderId)
        {
 
           return _db.Orders
            .Include(x => x.Activities)
            .ThenInclude(a => a.TradeDrugs)
            .FirstOrDefaultAsync(x => x.OrderId == orderId);
        }
        public async Task SaveOrderAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }
    }
}
