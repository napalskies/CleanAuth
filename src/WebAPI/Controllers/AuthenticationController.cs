using Application.Common.DTO.Authentication;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public AuthenticationController(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var result = await _identityService.CreateAsync(registerRequest);

            if (result.Succeeded)
            {
                return Ok("User registered successfully.");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            await _identityService.ValidateUserAsync(loginRequest);

            //if validation fails, an exception will be thrown
            
            var roles = await _identityService.GetUserRolesAsync(loginRequest.Username);
            var userId = await _identityService.GetUserIdByUsernameAsync(loginRequest.Username);

            var jwtToken = _tokenService.GenerateJwt(loginRequest.Username, [.. roles]);

            var refreshToken = await _tokenService.StoreRefreshTokenAsync(loginRequest.Username, userId);

            return Ok(new TokenResponse { JwtToken = jwtToken, RefreshToken = refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            string newRefreshToken = await _tokenService.UpdateRefreshTokenAsync(refreshToken);

            if (string.IsNullOrEmpty(newRefreshToken))
            {
                return BadRequest("Invalid refresh token.");
            }

            var userId = await _tokenService.GetUserIdAsync(refreshToken);
            var username = await _identityService.GetUserIdByUsernameAsync(userId);
            var roles = await _identityService.GetUserRolesAsync(username);

            var newJwt = _tokenService.GenerateJwt(username, [.. roles]);

            return Ok(new TokenResponse { JwtToken = newJwt, RefreshToken = newRefreshToken });
        }

    }
}
