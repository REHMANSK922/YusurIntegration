using System.Net;
using System.Net.Http.Headers;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Services
{
    public partial class YusurApiClient
    {
        public async Task<ApiErrorResponseDto?> SubmitAuthorization(SubmitAuthorizationRequestDto request)
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
                _logger.LogInformation("Accepting order {OrderId}  ", request.orderId);
                // var url = _base.TrimEnd('/') + "/api/orderAccept";
                // 2. Attach the token to the request (Assuming _accessToken is set by EnsureAuthenticatedAsync)
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _http.PostAsJsonAsync("submitAuthorization", request);

                // 3. Handle Token Expiry (401)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired, retrying with fresh token...");

                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;

                    if (await EnsureAuthenticatedAsync())
                    {
                        // Retry
                        response = await _http.PostAsJsonAsync("submitAuthorization", request);
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

        public async Task<ApiErrorResponseDto?> ReSubmitAuthorization(ResubmitAuthorizationRequestDto request)
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

                var response = await _http.PostAsJsonAsync("resubmitAuthoirzation", request);

                // 3. Handle Token Expiry (401)
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Token expired, retrying with fresh token...");

                    await _tokenRepo.InvalidateTokenAsync();
                    _accessToken = null;

                    if (await EnsureAuthenticatedAsync())
                    {
                        // Retry
                        response = await _http.PostAsJsonAsync("resubmitAuthoirzation", request);
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
    }

}
