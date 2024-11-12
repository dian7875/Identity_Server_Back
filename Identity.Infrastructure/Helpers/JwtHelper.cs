// Infrastructure/Helpers/JwtHelper.cs
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyApp.Infrastructure.Helpers
{
    public class JwtHelper
    {
        private static readonly string SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                                                  ?? throw new Exception("JWT_SECRET_KEY no está configurada en las variables de entorno.");


        public static bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                if (!tokenHandler.CanReadToken(token))
                    throw new Exception("Token no válido.");

                var jwtToken = tokenHandler.ReadJwtToken(token);

                var expirationDate = jwtToken.ValidTo;
                if (expirationDate < DateTime.UtcNow)
                    throw new Exception("Token ha expirado.");

                if (jwtToken.Issuer != "https://localhost:7222/")
                    throw new Exception("Emisor no válido.");
                if (jwtToken.Audiences.FirstOrDefault() != "api1")
                    throw new Exception("Audiencia no válida.");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "https://localhost:7222/",
                    ValidAudience = "api1",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                };

                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token no válido: {ex.Message}");
                return false;
            }
        }
    }
}
