﻿using System.Threading.Tasks;
using Atheer.Controllers.User.Models;
using Atheer.Models;
using Atheer.Services.OAuthService;

namespace Atheer.Services.UsersService
{
    public interface IUserService
    {
        Task<string> Add(RegisterViewModel registerViewModel);
        Task<string> AddOrUpdateOAuthUser(OAuthUserInfo oAuthUserInfo);
        Task<bool> EmailRegistered(string email);
        Task<User> Get(string id);
        Task<User> GetFromEmailOrUsernameForLogin(string emailOrUsername);
        Task SetLogin(string id);
        Task<bool> Exists(string newAuthorId);
        Task Update(string id, UserSettingsUpdate settingsViewModel);
        Task UpdatePassword(string id, string oldPassword, string newPassword);

        Task SetImage(string id, string imageUrl);

        Task<bool> HasRole(string id, string role);
        Task ChangeRole(string id, string role);
    }
}