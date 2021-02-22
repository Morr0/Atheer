using System;
using System.Linq;
using Atheer.Controllers.ViewModels;
using Atheer.Extensions;
using Atheer.Models;
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

            user.DateCreated = DateTime.UtcNow.GetString();

            user.PasswordHash = HashPassword(registerViewModel.Password);

            user.Roles = DefaultRole();

            return user;
        }

        internal string Id(string email)
        {
            // Before the @
            return email.Split('@')[0];
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

        public string AnotherId(string id)
        {
            return $"{id}-{DateTime.UtcNow.Minute.ToString()}";
        }

        public void TakeRole(ref User user, string role)
        {
            var roles = user.Roles.Split(',').Where(x => x != role);
            user.Roles = string.Join(',', roles);
        }

        public void AddRole(ref User user, string role)
        {
            if (user.Roles.Contains(role)) return;

            user.Roles = $"{user.Roles},{role}";
        }
    }
}