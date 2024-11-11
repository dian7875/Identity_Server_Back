using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string Cedula { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Lastname1 { get; set; }
        public string Lastname2 { get; set; }
        public string Password { get; set; }

        public int rolId { get; set; }
    }

}
