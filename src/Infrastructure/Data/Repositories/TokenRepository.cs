using Application.Common.DTO.Security;
using Application.Common.Models;
using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationDbContext _context;

        public TokenRepository(ApplicationDbContext context)
        {
            _context = context;
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

        public void UpdateRefreshToken(RefreshToken oldToken, RefreshToken newToken)
        {
            var existingToken = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == oldToken.Token && rt.UserId == oldToken.UserId);
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
