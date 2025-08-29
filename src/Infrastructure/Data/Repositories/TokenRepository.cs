using Application.Common.Models;
using Application.Interfaces.Repositories;
using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationDbContext _context;

        public TokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (token == null) return null;
            return new RefreshToken
            {
                Token = token.Token,
                UserId = token.UserId,
                ExpiryDate = token.ExpiryDate,
                Revoked = token.Revoked
            };
        }
        public async Task StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            var refreshTokenEntity = new RefreshTokenEntity 
            {
                Token = refreshToken.Token,
                UserId = refreshToken.UserId,
                ExpiryDate = refreshToken.ExpiryDate,
                Revoked = refreshToken.Revoked
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
        }

        public async void UpdateRefreshToken(RefreshToken oldToken, RefreshToken newToken)
        {
            var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == oldToken.Token && rt.UserId == oldToken.UserId);
            var newTokenEntity = new RefreshTokenEntity
            {
                Id = existingToken.Id,
                Token = newToken.Token,
                UserId = newToken.UserId,
                ExpiryDate = newToken.ExpiryDate,
                Revoked = newToken.Revoked
            };
            _context.RefreshTokens.Update(newTokenEntity);
        }

        public void DeleteRefreshToken(RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
