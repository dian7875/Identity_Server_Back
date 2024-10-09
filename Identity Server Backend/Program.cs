
using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuración
var configuration = builder.Configuration;

// Agregar infraestructura y contexto de base de datos
builder.Services.AddInfrastructure(configuration);

// Registro de servicios de aplicación
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRolService, RolService>();

// Add services to the container.
builder.Services.AddIdentityServer()
    .AddInMemoryClients(Clients.Get())
    .AddInMemoryApiResources(ApiResources.Get())
    .AddInMemoryApiScopes(ApiScopes.Get())
    .AddDeveloperSigningCredential();

builder.Services.AddControllers();


// Agregar JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7147";  // La URL de tu IdentityServer
        options.RequireHttpsMetadata = false;          // Solo para desarrollo, en producción deberías usar HTTPS
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                      // Validar quién emite el token
            ValidateAudience = false,                   // No es necesario validar la audiencia en este ejemplo
            ValidateLifetime = true,                    // Validar que el token no haya expirado
            ValidateIssuerSigningKey = true,            // Validar la firma del emisor
            ValidIssuer = "https://localhost:7147",     // El emisor del token, que debe coincidir con el servidor de autorización
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YourSecretKey"))  // La clave usada para firmar los tokens
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
