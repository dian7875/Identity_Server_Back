using Identity.Domain.entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Identity.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Rol> Roles { get; set; }
       


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relación uno a muchos entre User y Rol si es necesario
            modelBuilder.Entity<User>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RolId);

            modelBuilder.Entity<Rol>().HasData(
              new Rol { Id = 1, Name = "Admin", Description = "Rol de administrador", IsActive = true },
              new Rol { Id = 2, Name = "Client", Description = "Rol del cliente", IsActive = true }
          );

            // Seed data para usuarios
            var passwordHasher = new PasswordHasher<User>();


            var user2 = new User
            {
                Id = 2,
                Cedula = "000000000",
                Name = "Adrian",
                Lastname1 = "Aguilar",
                Lastname2 = "Diaz",
                PasswordHash = HashPassword("adrian123"),
                Email = "adminsudo@gmail.com",
                Phone = "12098723",
                Address = "Rio grande",
                DateRegistered = DateTime.UtcNow,
                IsActive = true,
                RolId = 1
            };
            

            modelBuilder.Entity<User>().HasData( user2);
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
