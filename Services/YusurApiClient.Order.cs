using System.Net;
using System.Net.Http.Headers;
using YusurIntegration.DTOs;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Services
{
    public partial class YusurApiClient
    {
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
               // var url = _base.TrimEnd('/') + "/api/orderAccept";
                // 2. Attach the token to the request (Assuming _accessToken is set by EnsureAuthenticatedAsync)
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _http.PostAsJsonAsync("orderAccept", request);

                // 3. Handle Token Expiry (401)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired, retrying with fresh token...");

                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;

                    if (await EnsureAuthenticatedAsync())
                    {
                        // Retry
                        response = await _http.PostAsJsonAsync("orderAccept", request);
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
                //var url = _base.TrimEnd('/') + "/api/orderReject";
                var response = await _http.PostAsJsonAsync("orderReject", request);

                // Handle 401 Unauthorized - token expired
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired, retrying with fresh token");
                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;
                    if (await EnsureAuthenticatedAsync())
                    {
                        response = await _http.PostAsJsonAsync("orderReject", request);
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
        public async Task<ApiErrorResponseDto?> CancelOrderAsync(OrderCancelRequestDto request)
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
                //var url = _base.TrimEnd('/') + "/api/orderCancel";
                var response = await _http.PostAsJsonAsync("orderCancel", request);

                // Handle 401 Unauthorized - token expired
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired, retrying with fresh token");
                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;
                    if (await EnsureAuthenticatedAsync())
                    {
                        response = await _http.PostAsJsonAsync("orderCancel", request);
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
        public async Task<ApiErrorResponseDto?> ConfirmOrderPickupAsync(OrderConfirmPickup request)
        {
            // 1. Centralize the URL construction
          //  var url = _base.TrimEnd('/') + "/api/orderConfirmPickup";

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
                var response = await _http.PostAsJsonAsync("orderConfirmPickup", request);

                // Handle 401 Unauthorized - token expired
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired for Order {OrderId}, retrying with fresh token", request.orderId);

                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;

                    if (await EnsureAuthenticatedAsync())
                    {
                        // FIX: Use the same 'url' variable as the first call
                        response = await _http.PostAsJsonAsync("orderConfirmPickup", request);
                    }
                    else
                    {
                        _logger.LogError("Re-authentication failed for Order {OrderId}", request.orderId);
                        return new ApiErrorResponseDto(new List<ErrorDto>
                     {
                    new ErrorDto("Session expired and re-login failed.", false)
                });
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    // FIX: Updated log message to match method intent
                    _logger.LogInformation("✓ Order {OrderId} pickup confirmed successfully", request.orderId);
                    return null;
                }

                // Handle Business/API Errors
                var errorObject = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
                if (errorObject?.errors != null)
                {
                    foreach (var err in errorObject.errors)
                    {
                        _logger.LogWarning("Yusur Pickup Error: {Field} - {Message}", err.field, err.message);
                    }
                }
                return errorObject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while confirming pickup for order {OrderId}", request.orderId);
                return new ApiErrorResponseDto(new List<ErrorDto>
        {
            new ErrorDto($"Internal Error: {ex.Message}", false)
        });
            }
        }
        public async Task<DispenseSuccessResponse?> PrescriptionDispenseAsync(PrescriptionDispenseRequestDto request)
        {
            if (!await EnsureAuthenticatedAsync()) return null;

            try
            {

                // 3. Post ShippingAddress as JSON Body
                var response = await _http.PostAsJsonAsync("prescriptionDispense", request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired for Order {OrderId}, retrying with fresh token", request.PrescriptionReferenceNumber);

                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;

                    if (await EnsureAuthenticatedAsync())
                    {
                        // FIX: Use the same 'url' variable as the first call
                        response = await _http.PostAsJsonAsync("prescriptionDispense", request);
                    }
                    else
                    {
                        _logger.LogError("Re-authentication failed for Order {OrderId}", request.PatientNationalId);
                        return new DispenseSuccessResponse(null, new List<ErrorDto>
                                {
                                    new ErrorDto("Session expired and re-login failed.", false)
                                });
                    }
                }

                // 4. Handle Response
                var result = await response.Content.ReadFromJsonAsync<DispenseSuccessResponse>();

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
                return new DispenseSuccessResponse(null, new List<ErrorDto> { new ErrorDto(ex.Message, false)
                });
            }
        }

    }
}
