using System;
using Atheer.Controllers.Dtos;
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
            Id(ref user);

            user.DateCreated = DateTime.UtcNow.ToString();

            user.PasswordHash = HashPassword(registerViewModel.Password);

            user.Roles = DefaultRole();

            return user;
        }

        private void Id(ref User user)
        {
            // Before the @
            user.Id = user?.Email.Split('@')[0];
        }

        private string HashPassword(string textPassword)
        {
            return Crypt.HashPassword(textPassword, HashRounds);
        }

        private string DefaultRole()
        {
            return UserRoles.ViewerRole;
        }
    }
}