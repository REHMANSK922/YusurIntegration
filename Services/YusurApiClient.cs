using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using YusurIntegration.DTOs;
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
                            DateTime.UtcNow.AddDays(90)  // Token valid for 24 hours
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

        public async Task<ApiErrorResponseDto?> AcceptOrderAsync(OrderAcceptRequestDto request)
        {
            // 1. Ensure we have valid token
            if (!await EnsureAuthenticatedAsync())
            {
                _logger.LogError("Cannot accept order - authentication failed");
                // Return a custom error if you want the UI to know auth failed
                    return new ApiErrorResponseDto(new List<ErrorDto>
                                {
                                new ErrorDto("Authentication with Yusur failed.", false)
                                });
            }
            try
            {
                _logger.LogInformation("Accepting order {OrderId} with {Count} activities", request.orderId, request.activities.Count);
                var url = _base.TrimEnd('/') + "/api/orderAccept";
                // 2. Attach the token to the request (Assuming _accessToken is set by EnsureAuthenticatedAsync)
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _http.PostAsJsonAsync(url, request);

                // 3. Handle Token Expiry (401)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired, retrying with fresh token...");

                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;

                    if (await EnsureAuthenticatedAsync())
                    {
                        // Retry
                        response = await _http.PostAsJsonAsync(url, request);
                    }
                    else
                    {
                        _logger.LogError("Re-authentication failed");
                        return new ApiErrorResponseDto(new List<ErrorDto>
                                {
                                new ErrorDto("Session expired and re-login failed.", false)
                                });
                    }
                }

                // 4. Handle Success
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Order {OrderId} accepted successfully", request.orderId);
                    return null; // Null indicates success (no errors)
                }

                // 5. Handle Business Errors from Yusur
                var errorObject = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
                if (errorObject?.errors != null)
                {
                    foreach (var err in errorObject.errors)
                    {
                        _logger.LogWarning("Yusur Accept Error: {Field} - {Message}", err.field, err.message);
                    }
                }
                return errorObject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while accepting order {OrderId}", request.orderId);
                return new ApiErrorResponseDto(new List<ErrorDto>
                                {
                                new ErrorDto($"Internal Error: {ex.Message}", false)
                                });

            }
        }
        public async Task<ApiErrorResponseDto?> RejectOrderAsync(OrderRejectRequestDto request)
        {
            // Ensure we have valid token
            if (!await EnsureAuthenticatedAsync())
            {
                return new ApiErrorResponseDto(new List<ErrorDto>
                                {
                                new ErrorDto("Authentication with Yusur failed.", false)
                                });
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
                        _logger.LogError("Re-authentication failed");
                        return new ApiErrorResponseDto(new List<ErrorDto>
                          {
                          new ErrorDto("Session expired and re-login failed.", false)
                          });
                    }
                }
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✓ Order {OrderId} rejected", request.orderId);
                     return null;
                }
                var errorObject = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
                if (errorObject?.errors != null)
                {
                    foreach (var err in errorObject.errors)
                    {
                        _logger.LogWarning("Yusur Accept Error: {Field} - {Message}", err.field, err.message);
                    }
                }
                return errorObject;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting order");
                return new ApiErrorResponseDto(new List<ErrorDto>
                          {
                          new ErrorDto($"Internal Error: {ex.Message}", false)
                          });
            }
        }


        public async Task<ResponseRejectReason> GetRejectionReasons()
        {
            // 1. Ensure valid token before calling
            if (!await EnsureAuthenticatedAsync()) return null;

            try
            {
                // 2. Fix the double slash and set the URL
                var url = _base.TrimEnd('/') + "/api/getRejectionReasons";

                // 3. Attach Authorization header
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                // 4. Perform the GET request
                var response = await _http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // 5. Read and return the data
                    return await response.Content.ReadFromJsonAsync<ResponseRejectReason>();
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


        public async Task<DispenseResponse?> PrescriptionDispenseAsync(PrescriptionDispenseRequestDto request)
        {
            if (!await EnsureAuthenticatedAsync()) return null;

            try
            {
                // 1. Build Query String
                //var queryParams = new Dictionary<string, string?>
                //    {
                //        { "patientNationalId", request.PatientNationalId },
                //        { "prescriptionReferenceNumber", request.PrescriptionReferenceNumber },
                //        { "isPickup", request.IsPickup.ToString().ToLower() },
                //        { "deliveryPeriodId", request.DeliveryPeriodId },
                //        { "deliveryDate", request.DeliveryDate }
                //    };

                //var queryString = string.Join("&", queryParams
                //    .Where(x => !string.IsNullOrEmpty(x.Value))
                //    .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));

                //var url = $"{_base.TrimEnd('/')}/api/prescriptionDispense?{queryString}";


                var url = $"{_base.TrimEnd('/')}/api/prescriptionDispense";

                // 2. Setup Headers -- already done in EnsureAuthenticatedAsync
                //_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                // 3. Post ShippingAddress as JSON Body
                var response = await _http.PostAsJsonAsync(url, request);

                // 4. Handle Response
                var result = await response.Content.ReadFromJsonAsync<DispenseResponse>();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Prescription {request.PrescriptionReferenceNumber} dispensed. OrderId: {result?.orderId}");
                    return result;
                }

                _logger.LogWarning($"Dispense failed for {request.PrescriptionReferenceNumber}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Prescription Dispense");
                return new DispenseResponse(null, new List<ErrorDto> { new ErrorDto(ex.Message, false) });
            }
        }

    }
}
