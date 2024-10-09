using Identity.Domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> ValidateCredentials(string cedula, string password);
        Task<string> CreateToken(User user);
    }
}
