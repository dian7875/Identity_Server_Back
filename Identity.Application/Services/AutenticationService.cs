using Identity.Domain.entities;
using IdentityServer4.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Identity.Application.Interfaces;

namespace Identity.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityServerInteractionService _interactionService;

        public AuthenticationService(IIdentityServerInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public async Task<string> ValidateCredentials(string cedula, string password)
        {
            // Validación simple: en un sistema real, deberías consultar la base de datos
            if (cedula == "123456" && password == "password")
            {
                return await Task.FromResult("User found");
            }
            return null;
        }

        public async Task<string> CreateToken(User user)
        {
           
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{user.Lastname1} {user.Lastname2}"),
                new Claim(ClaimTypes.Email, user.Email),
            };

           
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            
            var token = new JwtSecurityToken(
                issuer: "https://localhost:7147", 
                audience: "api1",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), 
                signingCredentials: creds);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
