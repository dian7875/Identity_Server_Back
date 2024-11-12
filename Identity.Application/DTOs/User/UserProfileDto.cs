using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.User
{
    public class UserProfileDto
    {
      
        public string Name { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Lastname { get; set; }
        public string? Role { get; set; }
    }

}
