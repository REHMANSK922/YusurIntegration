using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.DTOs;
using YusurIntegration.Hubs;
using YusurIntegration.Services;
using YusurIntegration.Services.Interfaces;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Controllers
{
    [Route("api/webhooks")]
    [ApiController]
    public class YusurWebhookController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IOrderService _orders;
        private readonly SignatureValidationService _sig;
        private ILogger<YusurWebhookController> _logger;


        //private readonly ConnectionManager _connections;


        public YusurWebhookController(
            AppDbContext db,
            IOrderService orders,
            SignatureValidationService sig,
            ILogger<YusurWebhookController> logger

 
            )
        {
            _db = db;
            _orders = orders;
            _sig = sig;
            _logger = logger;
          

        }
        private bool ValidateSecret()
        {
            //if (!Request.Headers.TryGetValue("secret-key", out var secret)) return false;
            //return _sig.Validate(secret);
            if (!Request.Headers.TryGetValue("secret-key", out var secretHeader))
                return false;

            var incomingSecret = secretHeader.ToString();

            return _sig.Validate(incomingSecret);
        }
        [HttpPost("notifyNewOrder")]
        public async Task<IActionResult> NotifyNewOrder([FromBody] NewOrderDto dto)
        {
            if (!ValidateSecret()) return Unauthorized();
            await _orders.HandleNewOrderAsync(dto);
            return Ok(new { status = "received" });
        }
        [HttpPost("notifyOrderAllocation")]
        public async Task<IActionResult> NotifyOrderAllocation([FromBody] OrderAllocationDto dto)
        {
            if (!ValidateSecret()) return Unauthorized();

            var payload = System.Text.Json.JsonSerializer.Serialize(dto);
            await _db.WebhookLogs.AddAsync(new Models.WebhookLog { WebhookType = "notifyOrderAllocation", OrderId = dto.orderId, Payload = payload, BranchLicense = dto.branchLicense,Status = "ALLOCATED" });
            await _db.SaveChangesAsync();
            await _orders.HandleOrderAllocationAsync(dto);
            return Ok();
        }
        [HttpPost("notifyAuthorizationResponseReceived")]
        public async Task<IActionResult> NotifyAuthorizationResponseReceived([FromBody] AuthorizationResponseDto dto)
        {


            if (!ValidateSecret()) return Unauthorized();
            try
            {
                var success= await _orders.HandleAuthorizationResponseAsync(dto);
                if (!success.Success)
                {
                    return BadRequest("Request not found");
                }
            //  TRIGGER SIGNALR HERE

                //string? branchLicense = dto.branchLicense;
                //var connections = _connections.GetConnections(branchLicense);
                //if (connections.Length > 0)
                //{
                //   // await _hub.Clients.Clients(connections).SendAsync("AuthorizationResponseReceived", success.Data);

                //}
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical failure in Auth Webhook");
                return StatusCode(500, "Internal Server Error");
            }
            
            
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
