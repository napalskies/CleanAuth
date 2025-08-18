using Application.Common.Models;

namespace Application.Interfaces.Repositories
{
    public interface ITokenRepository
    {
        public Task StoreRefreshTokenAsync(RefreshToken refreshToken);
        public void UpdateRefreshToken(RefreshToken oldToken, RefreshToken newToken);
        public void DeleteRefreshToken(RefreshToken refreshToken);

    }
}
