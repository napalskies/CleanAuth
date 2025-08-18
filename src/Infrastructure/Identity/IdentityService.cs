using Application.Common.DTO.Authentication;
using Application.Common.Models;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<Result> CreateAsync(RegisterRequest registerRequest)
        {
            var result = new Result();

            if (string.IsNullOrEmpty(registerRequest.Username) || string.IsNullOrEmpty(registerRequest.Password))
            {
                result.Succeeded = false;
                result.Errors = new[] { "Username and password cannot be empty." };
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

        public async Task<Result> LoginAsync(LoginRequest loginRequest)
        {
            var result = new Result();

            if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                result.Succeeded = false;
                result.Errors = new[] { "Username and password cannot be empty." };
                return result;
            }

            var user = await _userManager.FindByNameAsync(loginRequest.Username);

            if (user == null)
            {
                result.Succeeded = false;
                result.Errors = ["User does not exist."];
                return result;
            }

            var loginResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

            if (!loginResult.Succeeded)
            {
                result.Succeeded = false;
                result.Errors = ["Password is incorrect."];
                return result;
            }

            await _signInManager.SignInAsync(user, true);

            return result;
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
