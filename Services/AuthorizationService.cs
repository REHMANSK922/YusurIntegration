using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using YusurIntegration.Data;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;
using YusurIntegration.Services.Interfaces;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ITokenRepository _tokenRepo;
        private readonly string? _base;
        
        private readonly ILogger<YusurApiClient> _logger;
        private readonly AppDbContext _db;

        public AuthorizationService(HttpClient http, ITokenRepository tokenRepo,
            IConfiguration config, ILogger<YusurApiClient> logger, 
            AppDbContext db)
        {
            _httpClient = http;
            _config = config;
            _logger = logger;
            _base = _config.GetValue<string>("Yusur:ApiBaseUrl");
            _tokenRepo = tokenRepo;
            _db = db;
        }
        /*
        public async Task<(bool Success, string Message)> SubmitAuthorizationAsync(string orderId)
        {
            var payload = new SubmitAuthorizationRequestDto(orderId);

            var url = _base.TrimEnd('/') + "/api/submitAuthorization";
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                var tokenObj = await _tokenRepo.GetValidTokenAsync();
                var token = tokenObj?.AccessToken;
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                request.Content = JsonContent.Create(payload);
                var response = await _http.SendAsync(request);
                if( response.IsSuccessStatusCode)
                {

                }
            }
        }
        public async Task<(bool Success, string Message)> ResubmitAuthorizationAsync(ResubmitAuthorizationRequestDto dto)
        {
            var url = _base.TrimEnd('/') + "/api/resubmitAuthoirzation";

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                var tokenObj = await _tokenRepo.GetValidTokenAsync();
                var token = tokenObj?.AccessToken;
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                request.Content = JsonContent.Create(dto);
                var response = await _http.SendAsync(request);
                return response.IsSuccessStatusCode;
            }



            //var response = await _http.PostAsJsonAsync(url,dto);
            //return response.IsSuccessStatusCode;
        }*/
        

           public async Task<(bool Success, string Message)> SubmitAuthorizationAsync(string orderId)
            {
            
              
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null) return (false, "Order not found in database.");

                var tokenObj = await _tokenRepo.GetValidTokenAsync();
                var token = tokenObj?.AccessToken;
            var url = _base.TrimEnd('/') + "/api/submitAuthorization";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);  
                request.Content = JsonContent.Create(new { orderId });  
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    order.Status = "WAITING_ERX_HUB_APPROVAL";
                    await _db.SaveChangesAsync();
                    return (true, "Authorization submitted successfully.");
                }

                return (false, "Yusur API rejected the authorization request.");
            }

           public async Task<(bool Success, string Message)> ResubmitAuthorizationAsync(ResubmitAuthorizationRequestDto dto)
            {
                var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.orderId);
                if (order == null) return (false, "Order not found.");

                var tokenObj = await _tokenRepo.GetValidTokenAsync();
                var token = tokenObj?.AccessToken;

            var url = _base.TrimEnd('/') + "/api/resubmitAuthoirzation";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token); 
                request.Content = JsonContent.Create(dto);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    order.Status = "AUTHORIZATION_RESUBMITTED";
                    await _db.SaveChangesAsync();
                    return (true, "Resubmission successful.");
                }
                return (false, "Resubmission failed at Yusur API.");
            }
           public async Task<(bool Success, Order UpdatedOrder)> HandleAuthorizationResponseAsync(AuthorizationResponseDto dto)
           {
            // 1. Audit Log (Requirement #1)
            var log = new Models.WebhookLog
            {
                WebhookType = "notifyAuthorizationResponseReceived",
                OrderId = dto.orderId,
                Payload = JsonSerializer.Serialize(dto),
                Status = dto.status,
                BranchLicense = dto.branchLicense
            };
            _db.WebhookLogs.Add(log);

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // 2. Fetch the existing order including activities from Firebird
                var order = await _db.Orders
                    .Include(o => o.Activities)
                    .FirstOrDefaultAsync(o => o.OrderId == dto.orderId);
                if (order == null) return (false, null);
                order.Status = dto.status;
                foreach (var activityDto in dto.activities)
                {
                    var activity = order.Activities.FirstOrDefault(a => a.ActivityId == activityDto.id);
                    if (activity != null)
                    {
                        activity.authStatus = activityDto.authStatus; // APPROVED or REJECTED
                        activity.rejectionReason = activityDto.rejectionReason;
                    }
                }
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, order);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
           }


    }

    } 

