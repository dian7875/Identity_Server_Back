using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.User
{
   public class UserEditDto
    {
        public int Id { get; set; }
        public string? Cedula { get; set; }
        public string? Name { get; set; }
        public string? Lastname1 { get; set; }
        public string? Lastname2 { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
