using Identity.Application.DTOs.Auth;
using Identity.Application.Interfaces;
using Identity.Domain.entities;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Server_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = await _userService.RegisterUser(registerDto);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResponse = await _userService.LoginUser(loginDto);
            if (loginResponse == null) return Unauthorized("Invalid credentials");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddHours(24)
            };

            Response.Cookies.Append("Sid", loginResponse.SessionId, cookieOptions);

            return Ok(new
            {
                
                loginResponse.SessionId
            });
        }
    }
}


