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

        public async Task<IEnumerable<RolResponseDto>> GetAllRoles(string name = null)
        {
            var query = _context.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            return await query
                .Select(r => new RolResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();
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
                Description = rolDto.Description
            };

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            return new RolResponseDto
            {
                Id = rol.Id,
                Name = rol.Name,
                Description = rol.Description
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

        public async Task DeleteRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return;

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();
        }
    }
}
