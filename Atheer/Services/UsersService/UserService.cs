using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Services.UsersService.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atheer.Services.UsersService
{
    public class UserService : IUserService
    {
        private readonly UserFactory _factory;
        private readonly Data _context;
        private readonly ILogger<UserService> _logger;

        public UserService(UserFactory factory, Data data, ILogger<UserService> logger)
        {
            _factory = factory;
            _context = data;
            _logger = logger;
        }

        public async Task Add(RegisterViewModel registerViewModel)
        {
            string lowerCaseEmail = registerViewModel.Email.ToLowerInvariant();
            if (await EmailRegistered(lowerCaseEmail).ConfigureAwait(false))
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
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new FailedOperationException();
            }
            
        }

        // Will generate a new id until one is non-already existent
        private async Task<string> GetIdUntilVacancyExists(string id)
        {
            do
            {
                if (!(await Exists(id).ConfigureAwait(false))) return id;
                
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
            string lowerCasedEmailOrUsername = emailOrUsername.ToLowerInvariant();
            return Get(IsEmail(lowerCasedEmailOrUsername) ? _factory.Id(lowerCasedEmailOrUsername) : lowerCasedEmailOrUsername);
        }

        public async Task SetLogin(string id)
        {
            var time = DateTime.UtcNow.GetString();

            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            _context.Entry(user).Property(x => x.DateLastLoggedIn).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<bool> Exists(string userId)
        {
            return _context.User.AsNoTracking().Where(x => x.Id == userId).AnyAsync();
        }

        private bool IsEmail(string emailOrUsername)
        {
            return emailOrUsername.Contains('@') && emailOrUsername.Contains('.');
        }
    }
}