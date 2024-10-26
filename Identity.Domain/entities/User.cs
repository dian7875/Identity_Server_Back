using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Cedula { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Lastname1 { get; set; }
        public string? Lastname2 { get; set; }
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow; //Obtiene fecha actual de registro
        //Indica que el usuario está activo al momento de registrarse.
        public bool IsActive { get; set; } = true;

        public string PasswordHash { get; set; }

        // One to one relation
        public int RolId { get; set; }
        public Rol Rol { get; set; }

    }
}
