using Application.Common.Helpers;
using Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Application.Interfaces.Repositories;
using Application.Common.Models;
using Infrastructure.Data.Entities;

namespace Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly ITokenRepository _tokenRepository;

        public TokenService(IOptions<JwtOptions> jwtOptions, ITokenRepository tokenRepository) {

            _jwtOptions = jwtOptions.Value;
            _tokenRepository = tokenRepository;
        }

        public string GenerateJwt(string username, IEnumerable<string> roles)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToArray();

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims = [.. claims, .. roleClaims];

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                expires: DateTime.Now.AddMinutes(_jwtOptions.ExpiryMinutes),
                claims: claims,
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> StoreRefreshTokenAsync(string username, string userId)
        {
            string token = GetRefreshToken();
            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiryDate = DateTime.Now.AddMinutes(15),
                Revoked = false
            };

            await _tokenRepository.StoreRefreshTokenAsync(refreshToken);
            return token;
        }

        public async Task<string> UpdateRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) {
                return string.Empty;
            }
            var token = await _tokenRepository.GetRefreshTokenAsync(refreshToken);
            if (token == null)
            {
                return string.Empty;
            }

            var newToken = new RefreshToken()
            {
                Token = GetRefreshToken(),
                UserId = token.UserId,
                ExpiryDate = DateTime.Now.AddDays(15),
                Revoked = false
            };

            _tokenRepository.UpdateRefreshToken(token, newToken);

            return newToken.Token;
        }


        public void DeleteRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserIdAsync(string refreshToken)
        {
            var token = await _tokenRepository.GetRefreshTokenAsync(refreshToken);
            if (token == null)
            {
                throw new Exception("Invalid refresh token");
            }
            return token.UserId;
        }

        private string GetRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
