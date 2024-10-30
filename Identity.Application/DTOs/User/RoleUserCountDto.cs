using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.User
{
    //conteo de usuarios por cada rol
    public class RoleUserCountDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int UserCount { get; set; }
    }
}
