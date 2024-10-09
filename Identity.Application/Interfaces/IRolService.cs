﻿using Identity.Application.DTOs.Rol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface IRolService
    {
        Task<RolResponseDto> GetRolById(int id);
        Task<IEnumerable<RolResponseDto>> GetAllRoles();
        Task<RolResponseDto> CreateRol(RolDto rolDto);
        Task UpdateRol(int id, RolDto rolDto);
        Task DeleteRol(int id);
    }
}