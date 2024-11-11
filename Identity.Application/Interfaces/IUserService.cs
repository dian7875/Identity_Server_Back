using Identity.Application.DTOs.Auth;
using Identity.Application.DTOs.Rol;
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
        Task<IEnumerable<UsersResponseDTO>> GetAllUsers(string cedula = null, int pageNumber = 1, int pageSize = 5);

        Task<User> RegisterUser(RegisterDto registerDto);
        Task<string> LoginUser(LoginDto loginDto);
        Task UpdateUserAsync(int id, UserEditDto userEditDto);
        Task DeleteUserAsync(int id);
        Task UpdateUserRoleAsync(int userId, int roleId);
        Task<StatisticsDto> GetStatisticsAsync();
        Task<IEnumerable<RoleUserCountDto>> GetUserCountPerRoleAsync();
        //desactivar y activar
        Task DeactivateUserAsync(int id);
        Task ReactivateUserAsync(int id);
    }
}