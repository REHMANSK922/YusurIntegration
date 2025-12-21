using YusurIntegration.Models;

namespace YusurIntegration.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task SaveOrderAsync(Order order);
        Task<Order> GetByOrderId(string orderId);
    }

}
