using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
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
            user.Id = await GetIdUntilVacancyExists(user.Id).ConfigureAwait(false);
            
            await _repository.Add(user).ConfigureAwait(false);

            return user;
        }

        // Will generate a new id until one is non-already existent
        private async Task<string> GetIdUntilVacancyExists(string id)
        {
            do
            {
                var user = await Get(id).ConfigureAwait(false);
                if (user is null) return id;
                
                id = _factory.AnotherId(id);
            } while (true);
        }

        public Task<bool> EmailRegistered(string email)
        {
            return _repository.Has(email);
        }

        public Task<User> Get(string id)
        {
            return _repository.Get(id);
        }

        public Task<User> GetFromEmail(string email)
        {
            return Get(_factory.Id(email));
        }
    }
}