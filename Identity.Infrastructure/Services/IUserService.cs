using System.Threading.Tasks;
using Identity.Domain.entities;

namespace Identity.Services
{
    public interface IUserService
    {
        Task<string> ValidateCredentials(string cedula, string password);
        Task<string> CreateToken(User user);
    }
}