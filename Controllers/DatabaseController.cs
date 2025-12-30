using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YusurIntegration.Repositories.Interfaces;
using YusurIntegration.Services;
using YusurIntegration.Services.Interfaces;

namespace YusurIntegration.Controllers
{
    [Route("api/Database")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<DatabaseController> _logger;

        public DatabaseController(
            IDatabaseService databaseService,
            ILogger<DatabaseController> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        // GET: api/database/license/{branchLicense}
       [HttpGet("license/{branchLicense}")]
        public async Task<IActionResult> GetByLicense(string branchLicense, [FromQuery] string? status = null)
        {
            try
            {
                var data = await _databaseService.GetAllDataByLicenseAsync(branchLicense, status);
                return Ok(new { Success = true, Data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data for license {License}", branchLicense);
                return StatusCode(500, new { Success = false, Message = "Error fetching data", Error = ex.Message });
            }
        }

        // GET: api/database/orders/license/{branchLicense}
        [HttpGet("orders/license/{branchLicense}")]
        public async Task<IActionResult> GetOrdersByLicense(string branchLicense, [FromQuery] string? status = null)
        {
            try
            {
                var orders = await _databaseService.GetOrdersByLicenseAsync(branchLicense, status);
                return Ok(new { Success = true, Orders = orders, Count = orders.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for license {License}", branchLicense);
                return StatusCode(500, new { Success = false, Message = "Error fetching orders", Error = ex.Message });
            }
        }

        // GET: api/database/activities/license/{branchLicense}
        [HttpGet("activities/license/{branchLicense}")]
        public async Task<IActionResult> GetActivitiesByLicense(string branchLicense, [FromQuery] string? status = null)
        {
            try
            {
                var activities = await _databaseService.GetActivitiesByLicenseAsync(branchLicense, status);
                return Ok(new { Success = true, Activities = activities, Count = activities.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching activities for license {License}", branchLicense);
                return StatusCode(500, new { Success = false, Message = "Error fetching activities", Error = ex.Message });
            }
        }

        // GET: api/database/tradedrugs/license/{branchLicense}
        [HttpGet("tradedrugs/license/{branchLicense}")]
        public async Task<IActionResult> GetTradeDrugsByLicense(string branchLicense, [FromQuery] string? status = null)
        {
            try
            {
                var tradeDrugs = await _databaseService.GetTradeDrugsByLicenseAsync(branchLicense, status);
                return Ok(new { Success = true, TradeDrugs = tradeDrugs, Count = tradeDrugs.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching trade drugs for license {License}", branchLicense);
                return StatusCode(500, new { Success = false, Message = "Error fetching trade drugs", Error = ex.Message });
            }
        }

        // GET: api/database/counts/license/{branchLicense}
        [HttpGet("counts/license/{branchLicense}")]
        public async Task<IActionResult> GetRecordCounts(string branchLicense, [FromQuery] string? status = null)
        {
            try
            {
                var totalCount = await _databaseService.GetRecordCountsByLicenseAsync(branchLicense, status);

                // Get individual counts
                var orders = await _databaseService.GetOrdersByLicenseAsync(branchLicense, status);
                var activities = await _databaseService.GetActivitiesByLicenseAsync(branchLicense, status);
                var tradeDrugs = await _databaseService.GetTradeDrugsByLicenseAsync(branchLicense, status);

                return Ok(new
                {
                    Success = true,
                    TotalRecords = totalCount,
                    Breakdown = new
                    {
                        Orders = orders.Count(),
                        Activities = activities.Count(),
                        TradeDrugs = tradeDrugs.Count()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting counts for license {License}", branchLicense);
                return StatusCode(500, new { Success = false, Message = "Error getting counts", Error = ex.Message });
            }
        }

        // DELETE: api/database/orders/license/{branchLicense}
        [HttpDelete("orders/license/{branchLicense}")]
        public async Task<IActionResult> DeleteOrdersByLicense(string branchLicense, [FromQuery] string? status = null)
        {
            try
            {
                var count = await _databaseService.DeleteOrdersByLicenseAsync(branchLicense, status);
                _logger.LogInformation("Deleted {Count} orders for license {License} with status {Status}",
                    count, branchLicense, status ?? "all");

                return Ok(new { Success = true, Message = $"Deleted {count} orders", DeletedCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting orders for license {License}", branchLicense);
                return StatusCode(500, new { Success = false, Message = "Error deleting orders", Error = ex.Message });
            }
        }

        // DELETE: api/database/all/license/{branchLicense}
        [HttpDelete("all/license/{branchLicense}")]
        public async Task<IActionResult> DeleteAllByLicense(string branchLicense, [FromQuery] string? status = null)
        {
            try
            {
                var count = await _databaseService.DeleteAllDataByLicenseAsync(branchLicense, status);
                _logger.LogInformation("Deleted all data ({Count} records) for license {License} with status {Status}",
                    count, branchLicense, status ?? "all");

                return Ok(new { Success = true, Message = $"Deleted {count} total records", DeletedCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all data for license {License}", branchLicense);
                return StatusCode(500, new { Success = false, Message = "Error deleting data", Error = ex.Message });
            }
        }

        // POST: api/database/export/license/{branchLicense}
        [HttpPost("export/license/{branchLicense}")]
        public async Task<IActionResult> ExportData(string branchLicense, [FromQuery] string? status = null, [FromQuery] string format = "json")
        {
            try
            {
                var data = await _databaseService.GetAllDataByLicenseAsync(branchLicense, status);

                if (format.ToLower() == "csv")
                {
                    // You can implement CSV export here
                    return Ok(new { Success = true, Message = "CSV export not implemented yet", Data = data });
                }
                else
                {
                    return Ok(new { Success = true, Data = data });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting data for license {License}", branchLicense);
                return StatusCode(500, new { Success = false, Message = "Error exporting data", Error = ex.Message });
            }
        }

    }
}
    