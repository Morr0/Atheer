using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;

namespace Atheer.Services.UsersService
{
    public interface IUserService
    {
        Task Add(RegisterViewModel registerViewModel);
        Task<bool> EmailRegistered(string email);
        Task<User> Get(string id);
        Task<User> GetFromEmailOrUsername(string emailOrUsername);
        Task SetLogin(string id);
        Task<bool> Exists(string newAuthorId);
    }
}