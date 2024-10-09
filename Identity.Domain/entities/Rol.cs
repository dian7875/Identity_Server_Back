using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Relación uno a MUCHOS con User
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
