using Identity.Application.DTOs.Auth;
using Identity.Application.DTOs.User;
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
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> RegisterUser(RegisterDto registerDto);
        Task<string> LoginUser(LoginDto loginDto);
        Task UpdateUserAsync(int id, UserEditDto userEditDto);
        Task DeleteUserAsync(int id);
        Task<IEnumerable<UserDto>> GetUserAsync(int skip, int limit);
    }
}
