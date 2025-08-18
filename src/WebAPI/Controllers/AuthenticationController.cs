using Application.Common.DTO.Authentication;
using Application.Interfaces.Services;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            var result = await _identityService.LoginAsync(loginRequest);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var token = _tokenService.GenerateJwt(loginRequest);

            return Ok(token); 
        }

    }
}
