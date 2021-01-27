using System;
using System.Globalization;
using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Services.UserService.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Services.UserService
{
    public class UserService : IUserService
    {
        // private readonly UserRepository _repository;
        private readonly UserFactory _factory;
        private readonly Data _context;

        public UserService(UserRepository repository, UserFactory factory, Data data)
        {
            // _repository = repository;
            _factory = factory;
            _context = data;
        }
        
        public async Task Add(RegisterViewModel registerViewModel)
        {
            if (await EmailRegistered(registerViewModel.Email).ConfigureAwait(false))
            {
                throw new UserWithThisEmailAlreadyExistsException();
            }

            var user = _factory.Create(registerViewModel);
            user.Id = await GetIdUntilVacancyExists(user.Id).ConfigureAwait(false);

            await _context.User.AddAsync(user).ConfigureAwait(false);
            await _context.SaveChangesAsync();
            
            // await _repository.Add(user).ConfigureAwait(false);
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
            return _context.User.AnyAsync(x => x.Email == email);

            // return _repository.Has(email);
        }

        public Task<User> Get(string id)
        {
            return _context.User.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            // return _repository.Get(id);
        }

        public Task<User> GetFromEmailOrUsername(string emailOrUsername)
        {
            return Get(IsEmail(emailOrUsername) ? _factory.Id(emailOrUsername) : emailOrUsername);
        }

        public async Task SetLogin(string id)
        {
            var time = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            _context.Entry(user).Property(x => x.DateLastLoggedIn).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // return _repository.Set(id, nameof(User.DateLastLoggedIn), time);
        }

        private bool IsEmail(string emailOrUsername)
        {
            return emailOrUsername.Contains('@') && emailOrUsername.Contains('.');
        }
    }
}