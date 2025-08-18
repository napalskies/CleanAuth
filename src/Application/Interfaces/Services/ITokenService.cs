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
        string GenerateJwt(LoginRequest loginRequest);
        string GetRefreshToken();
        void StoreRefreshToken(string refreshToken);
        void UpdateRefreshToken(string refreshToken);
        void DeleteRefreshToken(string refreshToken);
    }
}
