using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.User.Models;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Services.OAuthService;
using Atheer.Services.UsersService.Exceptions;
using Atheer.Services.UsersService.Models.LoginAttempts;
using Atheer.Services.Utilities.TimeService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atheer.Services.UsersService
{
    public class UserService : IUserService
    {
        private static readonly int AttemptsUntilFreeze = 3;
        /// <summary>
        /// Window of time to freeze within if exceeded attempts
        /// </summary>
        private static readonly int FreezeWithinSecs = 120;
        /// <summary>
        /// How long to freeze
        /// </summary>
        private static readonly int FreezeTimeSecs = 180;
        
        private readonly UserFactory _factory;
        private readonly Data _context;
        private readonly ILogger<UserService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimeService _timeService;

        public UserService(UserFactory factory, Data data, ILogger<UserService> logger, IServiceScopeFactory serviceScopeFactory,
            ITimeService timeService)
        {
            _factory = factory;
            _context = data;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _timeService = timeService;
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

        public async Task TryLoginOAuth(string id)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id).CAF();
            user.LastLoggedInAt = _timeService.Get();
            
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<LoginAttemptResponse> TryLogin(string id, string rawPassword)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id).CAF();

            if (user.OAuthUser) throw new OAuthUserCannotLoginUsingPasswordException();
            
            (bool canLogin, int attemptsLeft) = await CanLogin(user).CAF();

            var time = _timeService.Get();
            if (!canLogin)
            {
                await AddLoginAttempt(user, time, false).CAF();
                await _context.SaveChangesAsync().CAF();
                
                return new FreezeLoginAttemptResponseResponse(user, time.AddSeconds(FreezeTimeSecs));
            }

            bool equalPassword = _factory.EqualPasswords(rawPassword, user.PasswordHash);
            if (equalPassword)
            {
                await AddLoginAttempt(user, time, true).CAF();
                user.LastLoggedInAt = time;
                await _context.SaveChangesAsync().CAF();

                return new ProceedLoginAttemptResponse(user);
            }

            await AddLoginAttempt(user, time, false).CAF();
            await _context.SaveChangesAsync().CAF();
            
            return new IncorrectCredentialsLoginAttemptResponse(user, attemptsLeft);
        }

        private async Task AddLoginAttempt(User user, DateTime time, bool successfulLogin)
        {
            var loginAttempt = new UserLoginAttempt
            {
                UserId = user.Id,
                AttemptAt = time,
                SuccessfulLogin = successfulLogin
            };

            await _context.UserLoginAttempt.AddAsync(loginAttempt).CAF();
        }

        private async Task<(bool canLogin, int attemptsLeft)> CanLogin(User user)
        {
            var lastNLoginAttempts = await _context.UserLoginAttempt.AsNoTracking()
                .Where(x => x.UserId == user.Id)
                .Where(x => !x.SuccessfulLogin)
                .OrderByDescending(x => x.AttemptAt)
                .Take(AttemptsUntilFreeze)
                .Select(x => x.AttemptAt)
                .ToListAsync().CAF();

            var since = _timeService.Get().AddSeconds(-1 * FreezeWithinSecs);
            int attemptedSince = lastNLoginAttempts.Count(attemptAt => attemptAt >= since);
            int attemptsLeft = AttemptsUntilFreeze - attemptedSince;
            
            return attemptsLeft > 0 ? (true, attemptsLeft) : (false, 0);
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