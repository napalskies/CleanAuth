using Application.Common.DTO.Authentication;
using Application.Common.Models;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

namespace Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private ITokenService _tokenService;

        public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<Result> CreateAsync(RegisterRequest registerRequest)
        {
            var result = new Result();

            if (string.IsNullOrEmpty(registerRequest.Username) || string.IsNullOrEmpty(registerRequest.Password))
            {
                result.Succeeded = false;
                result.Errors = ["Username and password cannot be empty."];
                return result;
            }

            var userExistsResult = await _userManager.FindByNameAsync(registerRequest.Username);

            if (userExistsResult != null)
            {
                result.Succeeded = false;
                result.Errors = ["Username already exists."];
                return result;
            }

            var user = new ApplicationUser { UserName = registerRequest.Username };

            var roleExistsResult = await this.RoleExistsAsync(registerRequest.Role);

            if (!roleExistsResult.Succeeded)
            {
                return roleExistsResult;
            }

            var createResult = await _userManager.CreateAsync(user, registerRequest.Password);

            if(!createResult.Succeeded)
            {
                result = new Result(createResult.Succeeded, createResult.Errors.Select(e => e.Description).ToArray());
                return result;
            }

            result = await this.AddToRoleAsync(user, registerRequest.Role);

            return result;

        }

        public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                throw new Exception("Username and password cannot be empty.");
            }

            var user = await _userManager.FindByNameAsync(loginRequest.Username);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var loginResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

            if (!loginResult.Succeeded)
            {
                throw new Exception("Invalid password.");
            }

            await _signInManager.SignInAsync(user, true);

            var roles = await _userManager.GetRolesAsync(user); 

            var jwtToken = _tokenService.GenerateJwt(loginRequest.Username, [.. roles]);

            var refreshToken = await _tokenService.StoreRefreshTokenAsync(loginRequest.Username, user.Id);

            return new TokenResponse { JwtToken = jwtToken, RefreshToken = refreshToken };
        }

        private async Task<Result> AddToRoleAsync(ApplicationUser user, string roleName)
        {
            var result = new Result();

            var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);


            if (!addToRoleResult.Succeeded)
            {
                result = new Result(false, addToRoleResult.Errors.Select(e => e.Description).ToArray());
            }

            return result;
        }

        private async Task<Result> RoleExistsAsync(string roleName)
        {
            var result = new Result();

            if (string.IsNullOrEmpty(roleName))
            {
                result.Succeeded = false;
                result.Errors = ["Role must be specified."];
                return result;
            }

            var roleExistsResult = await _roleManager.FindByNameAsync(roleName);

            if (roleExistsResult == null)
            {
                result.Succeeded = false;
                result.Errors = ["Role does not exist."];
                return result;
            }

            return result;
        }

        public async Task<string> GetUserIdByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            return user.Id;
        }

        public async Task<List<string>> GetUserRolesAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count == 0)
            {
                throw new ArgumentException("User has no roles assigned.");
            }
            
            return roles.ToList();
        }

    }
}
