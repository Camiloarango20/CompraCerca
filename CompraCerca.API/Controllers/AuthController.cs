using CompraCerca.API.DTOs;
using CompraCerca.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CompraCerca.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(UserCreateDto userDto)
        {
            var (response, errorMessage) = await _authService.RegisterAsync(userDto);

            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }

            return Ok(response);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto loginDto)
        {
            var (response, errorMessage) = await _authService.LoginAsync(loginDto);

            if (errorMessage != null)
            {
                return Unauthorized(errorMessage);
            }

            return Ok(response);
        }
    }
}