using YusurIntegration.Models;

namespace YusurIntegration.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task<ApiToken?> GetValidTokenAsync(string tokenType = "YUSUR");
        Task SaveTokenAsync(string tokenType, string accessToken, string username, DateTime? expiresAt = null);
        Task InvalidateTokenAsync(string tokenType = "YUSUR");
        Task UpdateLastUsedAsync(int tokenId);
    }
}
