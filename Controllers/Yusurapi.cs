using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YusurIntegration.Data;
using YusurIntegration.Models;
using YusurIntegration.Services;
using YusurIntegration.Services.Interfaces;

namespace YusurIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Yusurapi : ControllerBase
    {

        private ILogger<Yusurapi> _logger;
        private readonly YusurApiClient _yusurApiClient;

        public Yusurapi(
            ILogger<Yusurapi> logger,
            YusurApiClient yusurApiClient
            )
        {
            _logger = logger;
            _yusurApiClient = yusurApiClient;
        }
        [HttpGet("medications/{patientId}")]
        public async Task<IActionResult> Getactivemedications(string patientId)
        {
            try
            {
                var result = await _yusurApiClient.GetActiveMedicationsAsync(patientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("cities/{citycode}")]
        public async Task<IActionResult> GetCities(string citycode)
        {
            try
            {
                var result = await _yusurApiClient.GetCities(citycode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
