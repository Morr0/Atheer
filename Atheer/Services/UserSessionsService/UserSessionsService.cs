using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Services.UserService;

namespace Atheer.Services.UserSessionsService
{
    public class UserSessionsService : IUserSessionsService
    {
        private readonly UserFactory _userFactory;

        private Dictionary<string, UserSession> _sessions;

        public UserSessionsService(UserFactory userFactory)
        {
            _userFactory = userFactory;
            _sessions = new Dictionary<string, UserSession>();
        }
        
        public string Login(LoginViewModel loginViewModel, User user)
        {
            // TODO handle multiple devices in same session

            if (!_userFactory.EqualPasswords(loginViewModel.Password, user.PasswordHash)) return null;

            string sessionId = Guid.NewGuid().ToString();
            var session = new UserSession(sessionId, user.Id);
            lock (_sessions)
            {
                _sessions.Add(sessionId, session);
            }

            return sessionId;
        }

        public bool LoggedIn(string sessionId)
        {
            lock (_sessions)
            {
                return _sessions.ContainsKey(sessionId);
            }
        }

        public void Logout(string sessionId)
        {
            lock (_sessions)
            {
                _sessions.Remove(sessionId);
            }
        }
    }
}