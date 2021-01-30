using System;
using System.Globalization;
using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Services.UsersService.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Services.UsersService
{
    public class UserService : IUserService
    {
        private readonly UserFactory _factory;
        private readonly Data _context;

        public UserService(UserFactory factory, Data data)
        {
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

            await using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                await _context.User.AddAsync(user).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // TODO log transaction error
                throw new FailedOperationException();
            }
            
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
        }

        public Task<User> Get(string id)
        {
            return _context.User.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<User> GetFromEmailOrUsername(string emailOrUsername)
        {
            return Get(IsEmail(emailOrUsername) ? _factory.Id(emailOrUsername) : emailOrUsername);
        }

        public async Task SetLogin(string id)
        {
            var time = DateTime.UtcNow.GetString();

            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            _context.Entry(user).Property(x => x.DateLastLoggedIn).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        private bool IsEmail(string emailOrUsername)
        {
            return emailOrUsername.Contains('@') && emailOrUsername.Contains('.');
        }
    }
}