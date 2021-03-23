using System;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.User.Models;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Services.OAuthService;
using Atheer.Services.UsersService.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atheer.Services.UsersService
{
    public class UserService : IUserService
    {
        private readonly UserFactory _factory;
        private readonly Data _context;
        private readonly ILogger<UserService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserService(UserFactory factory, Data data, ILogger<UserService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _factory = factory;
            _context = data;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<string> Add(RegisterViewModel registerViewModel)
        {
            // Allow OAuth users with same email to register if exists or vice versa
            string lowerCaseEmail = registerViewModel.Email.ToLowerInvariant();
            var userEmail = await _context.User.AsNoTracking()
                .Where(x => x.Email == lowerCaseEmail && !x.OAuthUser)
                .Select(x => new { x.Email} ).FirstOrDefaultAsync().ConfigureAwait(false);
            if (userEmail?.Email == lowerCaseEmail)
            {
                throw new UserWithThisEmailAlreadyExistsException();
            }

            var user = _factory.Create(registerViewModel);
            user.Id = await GetIdUntilVacancyExists(user.Id).ConfigureAwait(false);

            await AddUser(user).ConfigureAwait(false);

            return user.Id;
        }

        public async Task<(string userId, string roles)> AddOrUpdateOAuthUser(OAuthUserInfo oAuthUserInfo)
        {
            var user = _factory.Create(oAuthUserInfo);
            var potentialSameExistingUser = await Get(user.Id);
            if (potentialSameExistingUser is null)
            {
                await AddUser(user).ConfigureAwait(false);
                return (user.Id, user.Roles);
            }
            
            // Same OAuth user then update
            if (potentialSameExistingUser.OAuthLogicalId == user.OAuthLogicalId)
            {
                user = _factory.UpdateOAuthUser(oAuthUserInfo, potentialSameExistingUser);
                _context.Attach(user);
                _context.Update(user);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            // Different OAuth user with same Id, resolve to different id
            else
            {
                user.Id = _factory.AnotherId(user.Id, user.OAuthProvider);
                await AddUser(user).ConfigureAwait(false);
            }

            return (user.Id, user.Roles);
        }

        private async Task AddUser(User user)
        {
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
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<User> GetFromEmailOrUsernameForLogin(string emailOrUsername)
        {
            string lowerCasedEmailOrUsername = emailOrUsername.ToLowerInvariant();
            return GetForLogin(IsEmail(lowerCasedEmailOrUsername) ? _factory.Id(lowerCasedEmailOrUsername) : lowerCasedEmailOrUsername);
        }

        private Task<User> GetForLogin(string id)
        {
            return _context.User.AsNoTracking()
                .Where(x => x.Id == id && !x.OAuthUser)
                .FirstOrDefaultAsync();
        }

        public async Task SetLogin(string id)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            user.DateLastLoggedIn = DateTime.UtcNow.GetString();
            
            _context.Entry(user).Property(x => x.DateLastLoggedIn).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<bool> Exists(string userId)
        {
            return _context.User.AsNoTracking().Where(x => x.Id == userId).AnyAsync();
        }

        public async Task Update(string id, UserSettingsUpdate settingsViewModel)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

            using var scope = _serviceScopeFactory.CreateScope();
            var mapper = scope.ServiceProvider.GetService<IMapper>();
            
            mapper.Map(settingsViewModel, user);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        private bool IsEmail(string emailOrUsername)
        {
            return emailOrUsername.Contains('@') && emailOrUsername.Contains('.');
        }

        public async Task UpdatePassword(string id, string oldPassword, string newPassword)
        {
            var userPassword = await _context.User.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

            if (userPassword == null) return;

            if (!_factory.EqualPasswords(oldPassword, userPassword.PasswordHash)) return;

            userPassword.PasswordHash = _factory.HashPassword(newPassword);

            _context.Entry(userPassword).Property(x => x.PasswordHash).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SetImage(string id, string imageUrl)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

            user.ImageUrl = imageUrl;

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<bool> HasRole(string id, string role)
        {
            string roles = await _context.User.AsNoTracking().Where(x => x.Id == id).Select(x => x.Roles)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return roles.Contains(role);
        }

        public async Task ChangeRole(string id, string role)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            
            // Downgrade from editor to basic
            if (role == UserRoles.BasicRole) _factory.TakeRole(user, UserRoles.EditorRole);
            else if (role == UserRoles.EditorRole) _factory.AddRole(user, UserRoles.EditorRole);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}