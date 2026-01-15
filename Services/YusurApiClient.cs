using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using YusurIntegration.DTOs;
using YusurIntegration.Hubs;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;
using YusurIntegration.Services.Interfaces;
using static YusurIntegration.DTOs.YusurPayloads;
namespace YusurIntegration.Services
{
    public partial class YusurApiClient: IYusurApiClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ITokenRepository _tokenRepo;
        private readonly string? _base;
        private string? _accessToken;
        private readonly ILogger<YusurApiClient> _logger;
        private int? _currentTokenId;
        private readonly SemaphoreSlim _authLock = new SemaphoreSlim(1, 1);
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
            _base = _config.GetValue<string>("YusurKeys:ApiBaseUrl");
            
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

                var response = await _http.PostAsJsonAsync("login", loginRequest);

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
                            DateTime.UtcNow.AddDays(90)  // Token valid for 24 hours
                        );
                        // Get the token ID
                        var savedToken = await _tokenRepo.GetValidTokenAsync();
                        _currentTokenId = savedToken?.Id;
                        _logger.LogInformation("Login successful, token saved to database");
                        return true;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Login failed: {Error}", errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return false;
            }
        }

       

        public async Task<List<ResponseRejectReason>> GetRejectionReasons()
        {
            // 1. Ensure valid token before calling
            if (!await EnsureAuthenticatedAsync()) return null;

            try
            {
                // 2. Fix the double slash and set the URL
                // var url = _base.TrimEnd('/') + "/api/getRejectionReasons";
                var url = "getRejectionReasons";
                // 3. Attach Authorization header
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                // 4. Perform the GET request
                var response = await _http.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    // 5. Read and return the data
                    return await response.Content.ReadFromJsonAsync<List<ResponseRejectReason>>();
                }
                _logger.LogWarning($"Failed to fetch rejection reasons. Status: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejection reasons from Yusur");
                return null;
            }
        }
        


 
        public async Task<bool> EnsureAuthenticatedAsync_old()
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

        public async Task<bool> EnsureAuthenticatedAsync()
        {
            // Wait for the lock to ensure only one login happens at a time
            await _authLock.WaitAsync();

            if (_http.BaseAddress == null)
            {
                // _base is your "https://api.dev2.wasfatyplus.com/integration-api/api/"
                _http.BaseAddress = new Uri(_base.TrimEnd('/') + "/");
            }
            try
            {
                // 1. Check if we already have token in memory
                if (!string.IsNullOrEmpty(_accessToken))
                {
                    if (_currentTokenId.HasValue)
                    {
                        await _tokenRepo.UpdateLastUsedAsync(_currentTokenId.Value);
                    }
                    return true;
                }

                // 2. Try to get token from database
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

                // 3. No valid token found - perform fresh login
                _logger.LogInformation("No valid token found, logging in...");
                return await LoginAsync();
            }
            catch (Exception ex)
            {
                // THE CATCH BLOCK: This handles DB errors or unexpected logic crashes
                _logger.LogError(ex, "Critical error during authentication check");
                return false;
            }
            finally
            {
                // ALWAYS release the lock in a finally block so other threads can enter
                _authLock.Release();
            }
        }
    
        public async Task<List<ActiveMedicationResponse>?> GetActiveMedicationsAsync(string patientId)
        {
            if (!await EnsureAuthenticatedAsync()) return null;
            try
            {
                var url = $"getActiveMedications?patientIdentifier={patientId}";
                var response = await _http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<ActiveMedicationResponse>>();
                    _logger.LogInformation("Fetched active medications successfully");
                    return result;
                }
                _logger.LogWarning($"Failed to fetch active medications. Status: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active medications from Yusur");
                return null;
            }
        }

        public async Task<List<DeliveryPeriod>> GetDeliveryPeriodsAsync()
        {
            if (!await EnsureAuthenticatedAsync()) return  new List<DeliveryPeriod>(); 
            try
            {
                //var url = $"{_base.TrimEnd('/')}/api/getDeliveryPeriods";
                var response = await _http.GetAsync("getDeliveryPeriods");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<DeliveryPeriod>>() ?? new List<DeliveryPeriod>();
                    _logger.LogInformation("Fetched active medications successfully");
                    return result;
                }
                _logger.LogWarning($"Failed to fetch active medications. Status: {response.StatusCode}");
                return new List<DeliveryPeriod>(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active medications from Yusur");
                return null;
            }
        }
        public async Task<List<City>> GetCities(string cityname)
        {
            if (!await EnsureAuthenticatedAsync()) return new List<City>();
            try
            {
                
                var response = await _http.GetAsync("getCities?name={cityname}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<City>>() ?? new List<City>() ;
                    _logger.LogInformation("Fetched active medications successfully");
                    return result;
                }
                _logger.LogWarning($"Failed to fetch active medications. Status: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active medications from Yusur");
                return null;
            }
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

    //public async Task<ApiErrorResponseDto?> AcceptOrderAsync(OrderAcceptRequestDto request)
    //{
    //    // Ensure we have valid token
    //    if (!await EnsureAuthenticatedAsync())
    //    {
    //        _logger.LogError("Cannot accept order - authentication failed");
    //        return false;
    //    }

    //    try
    //    {
    //        _logger.LogInformation(
    //            "Accepting order {OrderId} with {Count} activities",
    //            request.orderId,
    //            request.activities.Count);
    //        var url = _base.TrimEnd('/') + "/api/orderAccept";
    //        var response = await _http.PostAsJsonAsync(url, request);

    //        // Check if token expired (401 Unauthorized)
    //        if (response.StatusCode == HttpStatusCode.Unauthorized)
    //        {
    //            _logger.LogWarning("Token expired, invalidating and retrying with fresh token");

    //            // Invalidate current token
    //            await _tokenRepo.InvalidateTokenAsync();
    //           _accessToken = null;
    //           _currentTokenId = null;
    //            // Re-authenticate
    //            if (await EnsureAuthenticatedAsync())
    //            {
    //                // Retry the request with new token
    //                _logger.LogInformation("Retrying with fresh token...");
    //                response = await _http.PostAsJsonAsync("/orderAccept", request);
    //            }
    //            else
    //            {
    //                _logger.LogError("Re-authentication failed");
    //                return null;
    //            }
    //        }

    //        if (response.IsSuccessStatusCode)
    //        {
    //            _logger.LogInformation("Order {OrderId} accepted", request.orderId);
    //            return null;
    //        }

    //        var errorObject = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
    //        foreach (var err in errorObject?.errors ?? new())
    //        {
    //            _logger.LogWarning($"Yusur Accept Error: {err.field} - {err.message}");
    //        }
    //        return errorObject;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error accepting order {OrderId}", request.orderId);
    //    }
    //}

    //public async Task ConfirmDispenseAsync(string orderId, object dto)
    //{
    //    var url = _base.TrimEnd('/') + "/order/dispense";
    //    await _http.PostAsJsonAsync(url, dto);
    //}
}
