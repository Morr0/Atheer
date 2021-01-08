using System;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
using AutoMapper;
using Crypt = BCrypt.Net.BCrypt;

namespace Atheer.Services.UserService
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

            user.Id = Id(user.Email);

            user.DateCreated = DateTime.UtcNow.ToString();

            user.PasswordHash = HashPassword(registerViewModel.Password);

            user.Roles = DefaultRole();

            return user;
        }

        internal string Id(string email)
        {
            // Before the @
            return email.Split('@')[0];
        }

        private string HashPassword(string textPassword)
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
    }
}