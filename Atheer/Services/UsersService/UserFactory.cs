using System;
using System.Linq;
using Atheer.Controllers.User.Models;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services.OAuthService;
using AutoMapper;
using Crypt = BCrypt.Net.BCrypt;

namespace Atheer.Services.UsersService
{
    public class UserFactory
    {
        private const int HashRounds = 11;
        
        private readonly IMapper _mapper;

        public UserFactory(IMapper mapper)
        {
            _mapper = mapper;
        }
        
        public User Create(RegisterViewModel registerViewModel)
        {
            var user = _mapper.Map<User>(registerViewModel);

            user.Email = user.Email.ToLowerInvariant();
            user.Id = Id(user.Email);

            user.CreatedAt = DateTime.UtcNow;

            user.PasswordHash = HashPassword(registerViewModel.Password);

            user.Roles = DefaultRole();

            return user;
        }

        public User Create(OAuthUserInfo oAuthUserInfo)
        {
            var user = _mapper.Map<User>(oAuthUserInfo);

            user.Id = oAuthUserInfo.OAuthUsername;
            user.Email = user.Email.ToLowerInvariant();

            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = string.Empty;

            user.Roles = DefaultRole();

            user.OAuthUser = true;
            user.OAuthLogicalId = OAuthLogicalId(oAuthUserInfo.OAuthProvider, oAuthUserInfo.OAuthProviderId);

            return user;
        }

        public User UpdateOAuthUser(OAuthUserInfo oAuthUserInfo, User existingUser)
        {
            existingUser.LastLoggedInAt = DateTime.UtcNow;
         
            // HERE GOES ANYTHING YOU WANT TO PULL FROM OAUTH PROVIDER TO UPDATE USER DETAILS IF NEEDED
            
            return existingUser;
        }

        internal string Id(string email)
        {
            // Before the @
            return email.Split('@')[0];
        }

        internal string OAuthLogicalId(string provider, string oAuthId)
        {
            return $"{provider}-{oAuthId}";
        }

        internal string HashPassword(string textPassword)
        {
            return Crypt.HashPassword(textPassword, HashRounds);
        }

        internal bool EqualPasswords(string rawPassword, string hashedPassword)
        {
            return Crypt.Verify(rawPassword, hashedPassword);
        }

        private string DefaultRole()
        {
            return UserRoles.BasicRole;
        }

        public string AnotherId(string id, string oAuthProvider = null)
        {
            return string.IsNullOrEmpty(oAuthProvider) ? $"{id}-{DateTime.UtcNow.Minute.ToString()}" : $"{oAuthProvider}-{id}";
        }

        public void TakeRole(User user, string role)
        {
            if (role == UserRoles.BasicRole) return;
            
            var roles = user.Roles.Split(',').Where(x => x != role);
            user.Roles = string.Join(',', roles);
        }

        public void AddRole(User user, string role)
        {
            if (user.Roles.Contains(role)) return;

            user.Roles = $"{user.Roles},{role}";
        }
    }
}