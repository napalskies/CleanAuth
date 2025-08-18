using Application.Common.DTO.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokensAsync(string username);
        Task<string> GenerateJwt(string username);
        string GetRefreshToken();
        Task<string> StoreRefreshToken(string username);
        void UpdateRefreshToken(string refreshToken);
        void DeleteRefreshToken(string refreshToken);
    }
}
