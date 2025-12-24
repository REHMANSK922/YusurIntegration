using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;

namespace YusurIntegration.Repositories
{
    public class TokenRepository:ITokenRepository   
    {
        private readonly AppDbContext _db;
        private readonly ILogger<TokenRepository> _logger;

        public TokenRepository(AppDbContext db, ILogger<TokenRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<ApiToken?> GetValidTokenAsync(string tokenType = "YUSUR")
        {
            var token = await _db.ApiTokens
                .Where(t => t.TokenType == tokenType && t.IsValid)
                .OrderByDescending(t => t.CreatedDate)
                .FirstOrDefaultAsync();

            // Check if token is expired
            if (token != null && token.ExpiresAt.HasValue && token.ExpiresAt.Value < DateTime.UtcNow)
            {
                _logger.LogWarning("Token expired, marking as invalid");
                token.IsValid = false;
                await _db.SaveChangesAsync();
                return null;
            }

            return token;
        }

        public async Task SaveTokenAsync(
          string tokenType,
          string accessToken,
          string username,
          DateTime? expiresAt = null)
        {
            // Invalidate old tokens
            await InvalidateTokenAsync(tokenType);

            // Create new token
            var token = new ApiToken
            {
                TokenType = tokenType,
                AccessToken = accessToken,
                Username = username,
                CreatedDate = DateTime.UtcNow,
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddHours(24), // Default 24 hours
                IsValid = true,
                LastUsed = DateTime.UtcNow
            };

            _db.ApiTokens.Add(token);
            await _db.SaveChangesAsync();

            _logger.LogInformation("✓ Token saved for user {Username}", username);
        }
        public async Task InvalidateTokenAsync(string tokenType = "YUSUR")
        {
            var tokens = await _db.ApiTokens
                .Where(t => t.TokenType == tokenType && t.IsValid)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsValid = false;
            }

            await _db.SaveChangesAsync();
        }
        public async Task UpdateLastUsedAsync(int tokenId)
        {
            var token = await _db.ApiTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.LastUsed = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

    }

}
