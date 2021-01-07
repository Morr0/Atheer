﻿using System.Threading.Tasks;
using Atheer.Controllers.Dtos;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Services.UserService.Exceptions;

namespace Atheer.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserRepository _repository;
        private readonly UserFactory _factory;

        public UserService(UserRepository repository, UserFactory factory)
        {
            _repository = repository;
            _factory = factory;
        }
        
        public async Task<User> Add(RegisterViewModel registerViewModel)
        {
            if (await EmailRegistered(registerViewModel.Email).ConfigureAwait(false))
            {
                throw new UserWithThisEmailAlreadyExistsException();
            }

            var user = _factory.Create(registerViewModel);
            await _repository.Add(user).ConfigureAwait(false);

            return user;
        }

        public Task<bool> EmailRegistered(string email)
        {
            return _repository.Has(email);
        }

        public Task<User> Get(string id)
        {
            return _repository.Get(id);
        }
    }
}