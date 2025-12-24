using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.Services;
using YusurIntegration.Services.Interfaces;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {

        
        private readonly IAuthorizationService _authService;
        public AuthorizationController(IAuthorizationService authService)
        {
            _authService = authService;
        }
        [HttpPost("submit/{orderId}")]
        public async Task<IActionResult> Submit(string orderId)
        {
            var result = await _authService.SubmitAuthorizationAsync(orderId);

            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { error = result.Message });
        }

        [HttpPost("resubmit")]
        public async Task<IActionResult> Resubmit([FromBody] ResubmitAuthorizationRequestDto dto)
        {
            var result = await _authService.ResubmitAuthorizationAsync(dto);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { error = result.Message });
        }
    }
}
