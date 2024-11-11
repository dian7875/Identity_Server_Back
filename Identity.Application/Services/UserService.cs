﻿
using Identity.Application.DTOs.Auth;
using Identity.Application.Interfaces;
using Identity.Domain.entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Identity.Application.DTOs.User;



public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    public async Task<User> RegisterUser(RegisterDto registerDto)
    {
        // Verificar si el correo electrónico ya está registrado
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            throw new ArgumentException("El correo electrónico ya está en uso.");
        }
        // Verificar si la cédula ya está registrada
        if (await _context.Users.AnyAsync(u => u.Cedula == registerDto.Cedula))
        {
            throw new ArgumentException("La cédula ya está en uso.");
        }

        var user = new User
        {
            Cedula = registerDto.Cedula,
            Name = registerDto.Name,
            Email = registerDto.Email,
            Phone = registerDto.Phone,
            Address = registerDto.Address,
            Lastname1 = registerDto.Lastname1,
            Lastname2 = registerDto.Lastname2,
            PasswordHash = HashPassword(registerDto.Password),
            RolId =2,
            DateRegistered = DateTime.UtcNow, 
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    public async Task<string> LoginUser(LoginDto loginDto)
    {
        // Buscar al usuario por cédula y cargar el rol relacionado
        var user = await _context.Users
            .Include(u => u.Rol) // Incluir el rol relacionado
            .FirstOrDefaultAsync(u => u.Cedula == loginDto.Cedula);

        if (user == null) return null;

        // Validar la contraseña
        if (user.PasswordHash != HashPassword(loginDto.Password))
        {
            return null;
        }

        // Crear los claims, incluyendo el nombre del rol
        var claims = new[]
        {
        new Claim(ClaimTypes.Name, user.Name ?? ""),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Role, user.Rol?.Name ?? "")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyYourSecretKeyYourSecretKeyYourSecretKeyYourSecretKeyYourSecretKey"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "https://localhost:5001",
            audience: "api1",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);


        // Retornar tanto el token como el SessionId
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task<User> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("Usuario no encontrado.");
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task UpdateUserAsync(int id, UserEditDto userEditDto)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null) throw new KeyNotFoundException("Usuario no encontrado.");

        // Actualiza los datos de usuario
        existingUser.Cedula = userEditDto.Cedula;
        existingUser.Name = userEditDto.Name;
        existingUser.Lastname1 = userEditDto.Lastname1;
        existingUser.Lastname2 = userEditDto.Lastname2;
        existingUser.Email = userEditDto.Email;
        existingUser.Phone = userEditDto.Phone;
        existingUser.Address = userEditDto.Address;

         
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserRoleAsync(int userId, int roleId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado.");
        }

        var rol = await _context.Roles.FindAsync(roleId);
        if (rol == null)
        {
            throw new KeyNotFoundException("Rol no encontrado.");
        }

        user.RolId = roleId;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteUserAsync(int id)
    {
        //Busca por el id de usurio
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("Usuario no encontrado.");

        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<StatisticsDto> GetStatisticsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalRoles = await _context.Roles.CountAsync();

        return new StatisticsDto
        {
            TotalUsers = totalUsers,
            TotalRoles = totalRoles
        };
    }

    public async Task<IEnumerable<RoleUserCountDto>> GetUserCountPerRoleAsync()
    {
        return await _context.Roles
            .Select(r => new RoleUserCountDto
            {
                RoleId = r.Id,
                RoleName = r.Name,
                UserCount = _context.Users.Count(u => u.RolId == r.Id)
            })
            .ToListAsync();
    }

    //desactivar y activar
    public async Task DeactivateUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        user.IsActive = false;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task ReactivateUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        user.IsActive = true;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
