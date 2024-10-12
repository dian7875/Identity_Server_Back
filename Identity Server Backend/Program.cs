
using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder
            .WithOrigins("https://indentity-server-login.vercel.app",
            "http://localhost:5173",
            "http://localhost:5174",
            "https://user-manage-snowy.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});




// Cargar configuraci�n
var configuration = builder.Configuration;

// Agregar infraestructura y contexto de base de datos
builder.Services.AddInfrastructure(configuration);

// Registro de servicios de aplicaci�n
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
        options.RequireHttpsMetadata = false;          // Solo para desarrollo, en producci�n deber�as usar HTTPS
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                      // Validar qui�n emite el token
            ValidateAudience = false,                   // No es necesario validar la audiencia en este ejemplo
            ValidateLifetime = true,                    // Validar que el token no haya expirado
            ValidateIssuerSigningKey = true,            // Validar la firma del emisor
            ValidIssuer = "https://localhost:7147",     // El emisor del token, que debe coincidir con el servidor de autorizaci�n
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
app.UseCors("AllowSpecificOrigin");

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();