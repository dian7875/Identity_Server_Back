
using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyApp.Infrastructure.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .WithOrigins(
            "https://eshop-users.vercel.app",
            "https://eshop-loggin.vercel.app",
            "http://localhost:5174",
            "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


IdentityModelEventSource.ShowPII = true;


// Cargar configuraci�n
var configuration = builder.Configuration;
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // Configuración para el encabezado Authorization con el token JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",  // "bearer" indica el tipo de autenticación
        BearerFormat = "JWT",  // Este es el formato esperado para un token JWT
        In = ParameterLocation.Header,  // El token se incluirá en el encabezado
        Description = "JWT Authorization header usando el esquema Bearer. Incluye el token como: Bearer {token}"
    });

    // Añadir la configuración de seguridad para requerir el token JWT
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Relacionamos la seguridad con el esquema definido arriba
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                context.Fail("Token no recibido.");
                return Task.CompletedTask;
            }

            Console.WriteLine("Token recibido: " + token);

            if (JwtHelper.ValidateToken(token)) 
            {
                context.Token = token;
            }
            else
            {
                context.Fail("Token no válido.");
            }

            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://localhost:7222/", 
        ValidAudience = "api1",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyYourSecretKeyYourSecretKeyYourSecretKeyYourSecretKeyYourSecretKey"))
    };
});


builder.Services.AddAuthorization(options =>
{
    // Política para administradores
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    // Política para clientes
    options.AddPolicy("RequireClientRole", policy =>
        policy.RequireRole("Client"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Definir los roles que necesitas
    string[] roles = new string[] { "Admin", "Client" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}


app.UseHttpsRedirection();
app.UseCors("AllowAll");
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
