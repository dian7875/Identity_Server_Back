using Identity.Application.DTOs.Auth;
using Identity.Domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> RegisterUser(RegisterDto registerDto);
        Task<LoginResponseDto> LoginUser(LoginDto loginDto);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        
    }
}
