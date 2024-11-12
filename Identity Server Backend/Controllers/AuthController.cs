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
            var token = await _userService.LoginUser(loginDto);
            if (token == null) return Unauthorized("Invalid credentials");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddHours(24)
            };
            Response.Cookies.Append("jwt", token, cookieOptions);
            return Ok(new { Token = token });
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Elimina la cookie que contiene el token JWT
            HttpContext.Response.Cookies.Delete("jwt");

            return Ok(new { message = "Logout exitoso." });
        }
    }
  
}


