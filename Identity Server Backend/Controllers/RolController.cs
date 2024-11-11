using Identity.Application.DTOs;
using Identity.Application.DTOs.Rol;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity_Server_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolController(IRolService rolService)
        {

            _rolService = rolService;
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRolById(int id)
        {
            var rol = await _rolService.GetRolById(id);
            if (rol == null) return NotFound();
            return Ok(rol);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles([FromQuery] string name = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var roles = await _rolService.GetAllRoles(name, pageNumber, pageSize);
            var totalCount = await _rolService.GetAllRoles(name);

            return Ok(new
            {
                TotalCount = totalCount.Count(),
                page = pageNumber,
                limit = pageSize,
                Roles = roles
            });
        }


        [HttpPost]
        public async Task<IActionResult> CreateRol([FromBody] RolDto rolDto)
        {
            if (rolDto.IsActive == false || rolDto.IsActive == null)
            {
                rolDto.IsActive = true;
            }
            var rol = await _rolService.CreateRol(rolDto);
            return CreatedAtAction(nameof(GetRolById), new { id = rol.Id }, rol);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] RolDto rolDto)
        {
            await _rolService.UpdateRol(id, rolDto);
            return NoContent();
        }

        [HttpPatch("deactivate/{id}")]
        public async Task<IActionResult> DeactivateRol(int id)
        {
            await _rolService.DeactivateRol(id); 
            return NoContent();
        }

        [HttpPatch("reactivate/{id}")]
        public async Task<IActionResult> ReactivateRol(int id)
        {
            await _rolService.ReactivateRol(id);
            return NoContent();
        }
    }
}