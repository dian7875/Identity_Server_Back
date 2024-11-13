using Identity.Application.DTOs.Auth;
using Identity.Application.DTOs.User;
using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Domain.entities;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Server_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] string cedula = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {

            var totalCount = await _userService.GetAllUsers(cedula);

            var usuarios = await _userService.GetAllUsers(cedula, pageNumber, pageSize);

            return Ok(new
            {
                TotalCount = totalCount.Count(),
                Page = pageNumber,
                Limit = pageSize,
                Usuarios = usuarios
            });
        }


        //[Authorize(Policy = "RequireAdminRole")]
        //Crear usuario 
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.RegisterUser(registerDto);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserEditDto userEditDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userService.UpdateUserAsync(id, userEditDto);
                return NoContent(); // Error 204 No Content
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent(); //  Error 204 No Content
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize]

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            // Obtén la cédula desde los claims del usuario autenticado
            var cedula = User.Claims.FirstOrDefault(c => c.Type == "cedula")?.Value;

            if (string.IsNullOrEmpty(cedula))
            {
                return BadRequest("Cédula no encontrada en los claims del usuario.");
            }

            var userProfile = await _userService.GetUserProfileAsync(cedula);
            if (userProfile == null)
            {
                return NotFound();
            }

            var newToken = _userService.GenerateJwtToken(cedula, userProfile.Name, userProfile.Email, userProfile.Role);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1)
            };
            HttpContext.Response.Cookies.Append("jwt", newToken, cookieOptions);

            return Ok(userProfile);
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userService.UpdateUserRoleAsync(id, updateRoleDto.RoleId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("statistics/summary")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _userService.GetStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("statistics/users-per-role")]
        public async Task<IActionResult> GetUserCountPerRole()
        {
            try
            {
                var counts = await _userService.GetUserCountPerRoleAsync();
                return Ok(counts);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        [Authorize(Policy = "RequireAdminRole")]
        //activar y desactivar 
        [HttpPut("{id}/desactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                await _userService.DeactivateUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]

        [HttpPut("{id}/reactivate")]
        public async Task<IActionResult> ReactivateUser(int id)
        {
            try
            {
                await _userService.ReactivateUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize] 
        [HttpGet("user-info")]
        public IActionResult GetUserInfo()
        {
            var cedula = User.FindFirst("cedula")?.Value;

            return Ok(new { Cedula = cedula });
        }

    }
}