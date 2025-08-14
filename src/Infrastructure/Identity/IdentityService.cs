using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models;
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

        public Task<Result> LoginAsync(LoginRequest login)
        {
            throw new NotImplementedException();
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
    }
}
