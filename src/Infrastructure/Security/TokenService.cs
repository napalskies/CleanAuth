using Application.Common.DTO.Authentication;
using Application.Common.Helpers;
using Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;

        public TokenService(IOptions<JwtOptions> jwtOptions) {

            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateJwt(LoginRequest loginRequest)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginRequest.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                expires: DateTime.Now.AddMinutes(_jwtOptions.ExpiryMinutes),
                claims: claims,
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetRefreshToken()
        {
            throw new NotImplementedException();
        }

        public void StoreRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public void UpdateRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
        public void DeleteRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
