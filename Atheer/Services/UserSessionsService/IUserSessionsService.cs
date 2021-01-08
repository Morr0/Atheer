using System.Threading.Tasks;
using Atheer.Controllers.Dtos;
using Atheer.Models;

namespace Atheer.Services.UserSessionsService
{
    public interface IUserSessionsService
    {
        string Login(LoginViewModel loginViewModel, User user);
        bool LoggedIn(string sessionId);
        void Logout(string sessionId);
    }
}