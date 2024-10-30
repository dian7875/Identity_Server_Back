using Identity.Application.DTOs.Rol;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRolById(int id)
        {
            var rol = await _rolService.GetRolById(id);
            if (rol == null) return NotFound();
            return Ok(rol);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles([FromQuery] string name = null)
        {
            var roles = await _rolService.GetAllRoles(name);
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRol([FromBody] RolDto rolDto)
        {
            var rol = await _rolService.CreateRol(rolDto);
            return CreatedAtAction(nameof(GetRolById), new { id = rol.Id }, rol);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] RolDto rolDto)
        {
            await _rolService.UpdateRol(id, rolDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            await _rolService.DeleteRol(id);
            return NoContent();
        }
    }
}