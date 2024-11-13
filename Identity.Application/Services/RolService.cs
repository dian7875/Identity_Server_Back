using Identity.Application.DTOs.Rol;
using Identity.Application.Interfaces;
using Identity.Domain.entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Services
{
    public class RolService : IRolService
    {
        private readonly ApplicationDbContext _context;

        public RolService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RolResponseDto> GetRolById(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return null;

            return new RolResponseDto
            {
                Id = rol.Id,
                Name = rol.Name,
                Description = rol.Description
            };
        }

        public async Task<(List<RolResponseDto> Roles, int TotalCount)> GetAllRoles(string name = null, int pageNumber = 1, int pageSize = 10)

        {
            var query = _context.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }


            int totalCount = await query.CountAsync(); 

            var roles = await query
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize) 
                .Select(r => new RolResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return (Roles: roles, TotalCount: totalCount); 
        }


        public async Task<RolResponseDto> CreateRol(RolDto rolDto)
        {
            var rolExistente = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == rolDto.Name);


            if (rolExistente != null)
            {
                throw new InvalidOperationException($"El rol '{rolDto.Name}' ya existe.");
            }

            var rol = new Rol
            {
                Name = rolDto.Name,
                Description = rolDto.Description,
                IsActive = true
            };

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return new RolResponseDto
            {
                Id = rol.Id,
                Name = rol.Name,
                Description = rol.Description,
                IsActive = true
            };
        }

        public async Task UpdateRol(int id, RolDto rolDto)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return;

            rol.Name = rolDto.Name;
            rol.Description = rolDto.Description;
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateRol(int id) 
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return;

           
            rol.IsActive = false; 
            await _context.SaveChangesAsync();
        }
        public async Task ReactivateRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return;


            rol.IsActive = true;

            await _context.SaveChangesAsync();
        }
    }
}
