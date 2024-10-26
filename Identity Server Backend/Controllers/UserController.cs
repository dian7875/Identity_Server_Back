using Identity.Application.DTOs.Auth;
using Identity.Application.DTOs.User;
using Identity.Application.Interfaces;
using Identity.Domain.entities;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateUser([FromBody] User user)
        //{
        //    await _userService.AddUserAsync(user);
        //    return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        //}
     
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

        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            
            var profile = new
            {
                Name = User.Identity.Name,
                Email = User.FindFirst("email")?.Value,
                Lastname = $"{User.FindFirst("Lastname1")?.Value} {User.FindFirst("Lastname2")?.Value}"
            };
            return Ok(profile);
        }

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
    }
}