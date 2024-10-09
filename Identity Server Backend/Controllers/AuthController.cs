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

            return Ok(new { Token = token });
        }
    }
}


