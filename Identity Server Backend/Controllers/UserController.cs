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
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
        {
            // Obtener la lista de usuarios y aplicar paginación
            var users = await _userService.GetAllUsersAsync();

            // Calcular el total de usuarios antes de la paginación
            int totalUsers = users.Count();

            // Aplicar paginación
            var pagedUsers = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Retornar el resultado con información de paginación
            return Ok(new
            {
                TotalUsers = totalUsers,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                Data = pagedUsers
            });
        }
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


        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Identity.Application.DTOs.User.UserEditDto userEditDto)
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
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRolDto updateRoleDto)
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
    }
}