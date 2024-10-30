using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.User
{
    //conteo total de usuarios y roles.
    public class StatisticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
    }
}
