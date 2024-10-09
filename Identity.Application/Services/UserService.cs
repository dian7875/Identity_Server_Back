
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
            RolId = registerDto.rolId,
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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public Task<User> GetUserByIdAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(User user)
    {
        throw new NotImplementedException();
    }
}
