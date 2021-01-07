using System.Threading.Tasks;
using Atheer.Controllers.Dtos;
using Atheer.Models;

namespace Atheer.Services.UserService
{
    public interface IUserService
    {
        Task<User> Add(RegisterViewModel registerViewModel);
        Task<bool> EmailRegistered(string email);
        Task<User> Get(string id);
    }
}