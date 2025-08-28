using Application.Common.DTO.Authentication;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<Result> CreateAsync(RegisterRequest registerRequest);
        Task<TokenResponse> LoginAsync(LoginRequest loginRequest); 
        Task<string> GetUserIdByUsernameAsync(string username);
        Task<List<string>> GetUserRolesAsync(string username);
    }
}
