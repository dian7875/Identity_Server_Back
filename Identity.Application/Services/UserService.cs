
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
using Identity.Application.DTOs.Rol;
using static UserService;



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
            RolId = registerDto.rolId ?? 2,
            DateRegistered = DateTime.UtcNow, 
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    public string GenerateJwtToken(string cedula, string name, string email, string role)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.Name, name),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role),
        new Claim("cedula", cedula)
    };

        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new Exception("La clave secreta no está configurada.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "https://localhost:7222/",
            audience: "api1",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task<string> LoginUser(LoginDto loginDto)
    {
        var user = await _context.Users
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Cedula == loginDto.Cedula);

        if (user == null || user.PasswordHash != HashPassword(loginDto.Password))
        {
            return null;
        }

        return GenerateJwtToken(user.Cedula, user.Name ?? "", user.Email ?? "", user.Rol?.Name ?? "");
    }


    public async Task<UserProfileDto> GetUserProfileAsync(string cedula)
    {
        
        var user = await _context.Users
            .Where(u => u.Cedula == cedula)
            .Select(u => new UserProfileDto
            {
             
                Name = u.Name,
                Cedula = u.Cedula,
                Email = u.Email,
                Phone = u.Phone,
                Address = u.Address,
                Lastname = u.Lastname1,
                Role = u.Rol.Name // Asegúrate de que Role esté relacionado y sea accesible
            })
            .FirstOrDefaultAsync();

        return user;
    }


    public async Task<User> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("Usuario no encontrado.");
        return user;
    }

    public async Task<IEnumerable<UsersResponseDTO>> GetAllUsers(string cedula = null, int pageNumber = 1, int pageSize = 5)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(cedula))
        {
            query = query.Where(u => u.Cedula.Contains(cedula));
        }

        int totalCount = await query.CountAsync();

        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UsersResponseDTO
            {
                id = u.Id,
                name = u.Name,
                cedula = u.Cedula,
                email = u.Email,
                phone = u.Phone,
                address = u.Address,
                lastname1 = u.Lastname1,
                lastname2 = u.Lastname2,
                dateRegistered = u.DateRegistered,
                isActive = u.IsActive,
                rol = u.Rol.Name
            })
            .ToListAsync();

        return users;
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
