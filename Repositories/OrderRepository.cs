using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;

namespace YusurIntegration.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderRepository> _logger;
        public OrderRepository(AppDbContext db, ILogger<OrderRepository> logger)
        {
            _context = db;
            _logger = logger;
        }
        public Task<Order?> GetByOrderId(string orderId)
        {
 
           return _context.Orders
            .Include(x => x.Activities)
            .ThenInclude(a => a.TradeDrugs)
            .FirstOrDefaultAsync(x => x.OrderId == orderId);
        }
        public async Task SaveOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Order>> GetOrdersByLicenseAsync(string branchLicense, string? status = null)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Patient)
                    .Include(o => o.ShippingAddress)
                    .Include(o => o.Activities)
                        .ThenInclude(a => a.TradeDrugs)
                    .Where(o => o.BranchLicense == branchLicense);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(o => o.Status == status);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for license {License}", branchLicense);
                throw;
            }
        }

        public async Task<IEnumerable<Activity>> GetActivitiesByLicenseAsync(string branchLicense, string? status = null)
        {
            try
            {
                var query = _context.Activities
                    .Include(a => a.Order)
                    .Include(a => a.TradeDrugs)
                    .Where(a => a.Order.BranchLicense == branchLicense);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(a => a.Order.Status == status);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activities for license {License}", branchLicense);
                throw;
            }
        }

        public async Task<IEnumerable<TradeDrug>> GetTradeDrugsByLicenseAsync(string branchLicense, string? status = null)
        {
            try
            {
                var query = _context.TradeDrugs
                    .Include(t => t.Activity)
                        .ThenInclude(a => a.Order)
                    .Where(t => t.Activity.Order.BranchLicense == branchLicense);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(t => t.Activity.Order.Status == status);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trade drugs for license {License}", branchLicense);
                throw;
            }
        }

        public async Task<object> GetAllDataByLicenseAsync(string branchLicense, string? status = null)
        {
            try
            {
                var orders = await GetOrdersByLicenseAsync(branchLicense, status);
                var activities = await GetActivitiesByLicenseAsync(branchLicense, status);
                var tradeDrugs = await GetTradeDrugsByLicenseAsync(branchLicense, status);

                return new
                {
                    Orders = orders,
                    Activities = activities,
                    TradeDrugs = tradeDrugs,
                    Summary = new
                    {
                        TotalOrders = orders.Count(),
                        TotalActivities = activities.Count(),
                        TotalTradeDrugs = tradeDrugs.Count(),
                        StatusDistribution = orders.GroupBy(o => o.Status)
                            .Select(g => new { Status = g.Key, Count = g.Count() })
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all data for license {License}", branchLicense);
                throw;
            }
        }

        public async Task<int> DeleteOrdersByLicenseAsync(string branchLicense, string? status = null)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.BranchLicense == branchLicense &&
                               (status == null || o.Status == status))
                    .ToListAsync();

                _context.Orders.RemoveRange(orders);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting orders for license {License}", branchLicense);
                throw;
            }
        }

        public async Task<int> DeleteActivitiesByLicenseAsync(string branchLicense, string? status = null)
        {
            try
            {
                var activities = await _context.Activities
                    .Include(a => a.Order)
                    .Where(a => a.Order.BranchLicense == branchLicense &&
                               (status == null || a.Order.Status == status))
                    .ToListAsync();

                _context.Activities.RemoveRange(activities);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activities for license {License}", branchLicense);
                throw;
            }
        }

        public async Task<int> DeleteTradeDrugsByLicenseAsync(string branchLicense, string? status = null)
        {
            try
            {
                var tradeDrugs = await _context.TradeDrugs
                    .Include(t => t.Activity)
                        .ThenInclude(a => a.Order)
                    .Where(t => t.Activity.Order.BranchLicense == branchLicense &&
                               (status == null || t.Activity.Order.Status == status))
                    .ToListAsync();

                _context.TradeDrugs.RemoveRange(tradeDrugs);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting trade drugs for license {License}", branchLicense);
                throw;
            }
        }

        public async Task<int> DeleteAllDataByLicenseAsync(string branchLicense, string? status = null)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Delete in correct order to respect foreign key constraints
                var deletedTradeDrugs = await DeleteTradeDrugsByLicenseAsync(branchLicense, status);
                var deletedActivities = await DeleteActivitiesByLicenseAsync(branchLicense, status);
                var deletedOrders = await DeleteOrdersByLicenseAsync(branchLicense, status);

                await transaction.CommitAsync();

                return deletedOrders + deletedActivities + deletedTradeDrugs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all data for license {License}", branchLicense);
                throw;
            }
        }

        public async Task<int> GetRecordCountsByLicenseAsync(string branchLicense, string? status = null)
        {
            try
            {
                var ordersCount = await _context.Orders
                    .Where(o => o.BranchLicense == branchLicense &&
                               (status == null || o.Status == status))
                    .CountAsync();

                var activitiesCount = await _context.Activities
                    .Include(a => a.Order)
                    .Where(a => a.Order.BranchLicense == branchLicense &&
                               (status == null || a.Order.Status == status))
                    .CountAsync();

                var tradeDrugsCount = await _context.TradeDrugs
                    .Include(t => t.Activity)
                        .ThenInclude(a => a.Order)
                    .Where(t => t.Activity.Order.BranchLicense == branchLicense &&
                               (status == null || t.Activity.Order.Status == status))
                    .CountAsync();

                return ordersCount + activitiesCount + tradeDrugsCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting record counts for license {License}", branchLicense);
                throw;
            }
        }


    }
}
