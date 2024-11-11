using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.User
{
    public class UsersResponseDTO
    {
        public int id { get; set; }
        public string cedula { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string lastname1 { get; set; }
        public string lastname2 { get; set; }
        public DateTime dateRegistered { get; set; }
        public bool isActive { get; set; }
        public string rol { get; set; }

    }
}
