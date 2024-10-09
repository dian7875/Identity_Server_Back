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
        public async Task<IActionResult> GetUserById(User id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("all")]
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
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest("User ID mismatch");
            }

            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(User id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
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
    }
}
