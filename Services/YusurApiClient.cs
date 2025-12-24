using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using YusurIntegration.Hubs;
using YusurIntegration.Repositories.Interfaces;
using YusurIntegration.Services.Interfaces;
using static YusurIntegration.DTOs.YusurPayloads;
namespace YusurIntegration.Services
{
    public class YusurApiClient: IYusurApiClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ITokenRepository _tokenRepo;
        private readonly string? _base;
        private string? _accessToken;
        private readonly ILogger<YusurApiClient> _logger;
        private int? _currentTokenId;

        public YusurApiClient(
            HttpClient http, 
            ITokenRepository tokenRepo, 
            IConfiguration config, 
            ILogger<YusurApiClient> logger
            )
        {
            _http = http;
            _tokenRepo = tokenRepo;
            _config = config;
            _logger = logger;
            _base = _config.GetValue<string>("Yusur:ApiBaseUrl");
            
        }

        private async Task<bool> LoginAsync()
        {
            try
            {
                var username = _config["Yusur:Username"];
                var password = _config["Yusur:Password"];

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogError("Yusur credentials not configured");
                    return false;
                }

                var loginRequest = new LoginRequestDto(username, password);

                _logger.LogInformation("Attempting login for user: {Username}", username);

                var response = await _http.PostAsJsonAsync("/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    if (loginResponse?.accessToken != null)
                    {
                        _accessToken = loginResponse.accessToken;
                        _http.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", _accessToken);

                        // Save token to database
                        await _tokenRepo.SaveTokenAsync(
                            "YUSUR",
                            _accessToken,
                            username,
                            DateTime.UtcNow.AddHours(24)  // Token valid for 24 hours
                        );

                        // Get the token ID
                        var savedToken = await _tokenRepo.GetValidTokenAsync();
                        _currentTokenId = savedToken?.Id;

                        _logger.LogInformation("✓ Login successful, token saved to database");
                        return true;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("✗ Login failed: {Error}", errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return false;
            }
        }


        //public async Task AcceptOrderAsync(OrderAcceptRequestDto request)
        //{
        //    EnsureAuthenticated();
        //    var url = _base.TrimEnd('/') + "/order/accept";
        //    await _http.PostAsJsonAsync(url, request);
        //}

        //public async Task RejectOrderAsync(string orderId, string reason)
        //{
        //    EnsureAuthenticated();
        //    var url = _base.TrimEnd('/') + "/order/reject";
        //    await _http.PostAsJsonAsync(url, new { orderId, rejectionReason = reason });
        //}

        public async Task<bool> AcceptOrderAsync(OrderAcceptRequestDto request)
        {
            // Ensure we have valid token
            if (!await EnsureAuthenticatedAsync())
            {
                _logger.LogError("Cannot accept order - authentication failed");
                return false;
            }

            try
            {
                _logger.LogInformation(
                    "Accepting order {OrderId} with {Count} activities",
                    request.orderId,
                    request.activities.Count);
                var url = _base.TrimEnd('/') + "/api/orderAccept";
                var response = await _http.PostAsJsonAsync(url, request);

                // Check if token expired (401 Unauthorized)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired, invalidating and retrying with fresh token");

                    // Invalidate current token
                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;
                    _currentTokenId = null;

                    // Re-authenticate
                    if (await EnsureAuthenticatedAsync())
                    {
                        // Retry the request with new token
                        _logger.LogInformation("Retrying with fresh token...");
                        response = await _http.PostAsJsonAsync("/orderAccept", request);
                    }
                    else
                    {
                        _logger.LogError("Re-authentication failed");
                        return false;
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Order {OrderId} accepted", request.orderId);
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(" Failed to accept order: {Error}", errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting order {OrderId}", request.orderId);
                return false;
            }
        }
        public async Task<bool> RejectOrderAsync(OrderRejectRequestDto request)
        {
            // Ensure we have valid token
            if (!await EnsureAuthenticatedAsync())
            {
                return false;
            }

            try
            {
                var url = _base.TrimEnd('/') + "/api/orderReject";
                var response = await _http.PostAsJsonAsync(url, request);

                // Handle 401 Unauthorized - token expired
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired, retrying with fresh token");

                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;

                    if (await EnsureAuthenticatedAsync())
                    {
                        response = await _http.PostAsJsonAsync("/orderReject", request);
                    }
                    else
                    {
                        return false;
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✓ Order {OrderId} rejected", request.orderId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting order");
                return false;
            }
        }
        
        
        public async Task SendAwbAsync(string orderId, string awb)
        {
            var url = _base.TrimEnd('/') + "/order/awb";
            await _http.PostAsJsonAsync(url, new { orderId, awb });
        }

        public async Task ConfirmDispenseAsync(string orderId, object dto)
        {
            var url = _base.TrimEnd('/') + "/order/dispense";
            await _http.PostAsJsonAsync(url, dto);
        }
        //private void EnsureAuthenticated()
        //{
        //    if (string.IsNullOrEmpty(_accessToken))
        //    {
        //        throw new InvalidOperationException("Not authenticated");
        //    }
        //}
        public async Task<bool> EnsureAuthenticatedAsync()
        {
            // Check if we already have token in memory
            if (!string.IsNullOrEmpty(_accessToken))
            {
                // Update last used timestamp
                if (_currentTokenId.HasValue)
                {
                    await _tokenRepo.UpdateLastUsedAsync(_currentTokenId.Value);
                }
                return true;
            }

            // Try to get token from database
            var dbToken = await _tokenRepo.GetValidTokenAsync();

            if (dbToken != null && !string.IsNullOrEmpty(dbToken.AccessToken))
            {
                _logger.LogInformation("✓ Using token from database");
                _accessToken = dbToken.AccessToken;
                _currentTokenId = dbToken.Id;
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _accessToken);

                await _tokenRepo.UpdateLastUsedAsync(dbToken.Id);
                return true;
            }

            // No valid token - login
            _logger.LogInformation("No valid token found, logging in...");
            return await LoginAsync();
        }

    }
}
