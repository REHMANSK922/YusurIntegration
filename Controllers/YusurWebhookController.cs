using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.DTOs;
using YusurIntegration.Services;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Controllers
{
    [Route("api/webhooks")]
    [ApiController]
    public class YusurWebhookController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly OrderService _orders;
        private readonly SignatureValidationService _sig;

        public YusurWebhookController(
            AppDbContext db,
            OrderService orders,
            SignatureValidationService sig)
        {
            _db = db;
            _orders = orders;
            _sig = sig;
        }
        private bool ValidateSecret()
        {
            if (!Request.Headers.TryGetValue("secret-key", out var secret)) return false;
            return _sig.Validate(secret);
        }
        [HttpPost("notifyNewOrder")]
        public async Task<IActionResult> NotifyNewOrder([FromBody] NewOrderDto dto)
        {
            if (!ValidateSecret()) return Unauthorized();

            var payload = System.Text.Json.JsonSerializer.Serialize(dto);


            //dto.orderId

            await _db.WebhookLogs.AddAsync(new Models.WebhookLog { WebhookType = "notifyNewOrder", Payload = payload, BranchLicense = dto.branchLicense });
            await _db.SaveChangesAsync();

            await _orders.HandleNewOrderAsync(dto);


            return Ok(new { status = "received" });



        }
        [HttpPost("notifyOrderAllocation")]
        public async Task<IActionResult> NotifyOrderAllocation([FromBody] OrderAllocationDto dto)
        {
            if (!ValidateSecret()) return Unauthorized();

            var payload = System.Text.Json.JsonSerializer.Serialize(dto);
            await _db.WebhookLogs.AddAsync(new Models.WebhookLog { WebhookType = "notifyOrderAllocation", Payload = payload, BranchLicense = dto.branchLicense });
            await _db.SaveChangesAsync();
            await _orders.HandleOrderAllocationAsync(dto);
            return Ok();
        }
        [HttpPost("notifyAuthorizationResponseReceived")]
        public async Task<IActionResult> NotifyAuthorizationResponseReceived([FromBody] AuthorizationResponseDto dto)
        {
            if (!ValidateSecret()) return Unauthorized();

            var payload = System.Text.Json.JsonSerializer.Serialize(dto);
            await _db.WebhookLogs.AddAsync(new Models.WebhookLog { WebhookType = "notifyAuthorizationResponseReceived", Payload = payload, BranchLicense = dto.branchLicense });
            await _db.SaveChangesAsync();
            await _orders.HandleAuthorizationResponseAsync(dto);
            return Ok();
        }
        [HttpPost("notifyOrderStatusUpdate")]
        public async Task<IActionResult> NotifyOrderStatusUpdate([FromBody] StatusUpdateDto dto)
        {
            if (!ValidateSecret()) return Unauthorized();

            var payload = System.Text.Json.JsonSerializer.Serialize(dto);
            await _db.WebhookLogs.AddAsync(new Models.WebhookLog { WebhookType = "notifyOrderStatusUpdate", Payload = payload, BranchLicense = dto.branchLicense });
            await _db.SaveChangesAsync();

            await _orders.HandleStatusUpdateAsync(dto);
            return Ok();
        }



    }
}
