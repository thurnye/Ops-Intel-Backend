using OperationIntelligence.DB.Entities;

namespace OperationIntelligence.Core.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId);
        Task<RefreshToken> CreateAsync(RefreshToken token);
        Task InvalidateAsync(RefreshToken token);
        Task ReplaceAsync(RefreshToken oldToken, RefreshToken newToken);
        Task SaveChangesAsync();
    }
}
