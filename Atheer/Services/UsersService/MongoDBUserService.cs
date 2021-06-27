using System;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.User.Models;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services.OAuthService;
using Atheer.Services.UsersService.Exceptions;
using Atheer.Services.UsersService.Models.LoginAttempts;
using Atheer.Services.Utilities.TimeService;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Atheer.Services.UsersService
{
    public class MongoDBUserService : IUserService
    {
        private readonly UserFactory _userFactory;
        private readonly IMongoClient _client;
        private readonly ILogger<MongoDBUserService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimeService _timeService;
        private static readonly int AttemptsUntilFreeze = 3;
        /// <summary>
        /// Window of time to freeze within if exceeded attempts
        /// </summary>
        private static readonly int FreezeWithinSecs = 120;
        /// <summary>
        /// How long to freeze
        /// </summary>
        private static readonly int FreezeTimeSecs = 180;

        public MongoDBUserService(UserFactory userFactory, IMongoClient client, ILogger<MongoDBUserService> logger, 
            IServiceScopeFactory serviceScopeFactory, ITimeService timeService)
        {
            _userFactory = userFactory;
            _client = client;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _timeService = timeService;
        }
        
        public async Task<string> Add(RegisterViewModel registerViewModel)
        {
            // Allow OAuth users with same email to register if exists or vice versa
            string lowerCaseEmail = registerViewModel.Email.ToLowerInvariant();
            var existingUser =
                await (await _client.User().FindAsync(x => x.Email == lowerCaseEmail && !x.OAuthUser).CAF())
                    .FirstOrDefaultAsync().CAF();
            if (existingUser.Email == lowerCaseEmail)
            {
                throw new UserWithThisEmailAlreadyExistsException();
            }

            var user = _userFactory.Create(registerViewModel);
            user.Id = await GetIdUntilVacancyExists(user.Id).ConfigureAwait(false);

            await AddUser(user).ConfigureAwait(false);

            return user.Id;
        }
        
        // Will generate a new id until one is non-already existent
        private async Task<string> GetIdUntilVacancyExists(string id)
        {
            do
            {
                if (!(await Exists(id).ConfigureAwait(false))) return id;
                
                id = _userFactory.AnotherId(id);
            } while (true);
        }
        
        private async Task<bool> Exists(string userId)
        {
            return await (await _client.User().FindAsync(x => x.Id == userId).CAF()).AnyAsync().CAF();
        }
        
        private async Task AddUser(User user)
        {
            await _client.User().InsertOneAsync(user).CAF();
        }

        public async Task<(string userId, string roles)> AddOrUpdateOAuthUser(OAuthUserInfo oAuthUserInfo)
        {
            var user = _userFactory.Create(oAuthUserInfo);
            var potentialSameExistingUser = await Get(user.Id).CAF();
            if (potentialSameExistingUser is null)
            {
                await AddUser(user).CAF();
                return (user.Id, user.Roles);
            }
            
            // Same OAuth user then update
            if (potentialSameExistingUser.OAuthLogicalId == user.OAuthLogicalId)
            {
                user = _userFactory.UpdateOAuthUser(oAuthUserInfo, potentialSameExistingUser);
                await _client.User().FindOneAndReplaceAsync(x => x.OAuthLogicalId == user.OAuthLogicalId, user).CAF();
            }
            // Different OAuth user with same Id, resolve to different id
            else
            {
                user.Id = _userFactory.AnotherId(user.Id, user.OAuthProvider);
                await AddUser(user).ConfigureAwait(false);
            }

            return (user.Id, user.Roles);
        }

        public async Task<User> Get(string id)
        {
            return await (await _client.User().FindAsync(x => x.Id == id).CAF()).FirstOrDefaultAsync().CAF();
        }

        public async Task<LoginAttemptResponse> TryLogin(string id, string rawPassword)
        {
            var user = await (await _client.User().FindAsync(x => x.Id == id).CAF()).FirstOrDefaultAsync().CAF();

            if (user is null) return null;
            
            if (user.OAuthUser) throw new OAuthUserCannotLoginUsingPasswordException();
            
            (bool canLogin, int attemptsLeft) = await CanLogin(user).CAF();
            var time = _timeService.Get();
            
            bool equalPassword = _userFactory.EqualPasswords(rawPassword, user.PasswordHash);
            if (equalPassword)
            {
                await AddLoginAttempt(user, time, true).CAF();
                user.LastLoggedInAt = time;

                return new ProceedLoginAttemptResponse(user);
            }
            
            if (!canLogin)
            {
                await AddLoginAttempt(user, time, false).CAF();
                
                return new FreezeLoginAttemptResponseResponse(user, time.AddSeconds(FreezeTimeSecs));
            }

            await AddLoginAttempt(user, time, false).CAF();
            
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

            await _client.UserLoginAttempt().InsertOneAsync(loginAttempt).CAF();
        }
        
        private async Task<(bool canLogin, int attemptsLeft)> CanLogin(User user)
        {
            var lastNLoginAttempts = await (await _client.UserLoginAttempt().FindAsync(x =>
                x.UserId == user.Id && !x.SuccessfulLogin, new FindOptions<UserLoginAttempt>
            {
                Sort = Builders<UserLoginAttempt>.Sort.Descending(x => x.AttemptAt),
                Limit = AttemptsUntilFreeze
            }).CAF()).ToListAsync().CAF();

            var since = _timeService.Get().AddSeconds(-1 * FreezeWithinSecs);
            int attemptedSince = lastNLoginAttempts.Count(x => x.AttemptAt >= since);
            int attemptsLeft = AttemptsUntilFreeze - attemptedSince;
            
            return attemptsLeft > 0 ? (true, attemptsLeft) : (false, 0);
        }

        public async Task TryLoginOAuth(string id)
        {
            await _client.User().FindOneAndUpdateAsync(x => x.Id == id,
                Builders<User>.Update.Set(x => x.LastLoggedInAt, _timeService.Get())).CAF();
        }

        public async Task Update(string id, UserSettingsUpdate settingsViewModel)
        {
            var user = await (await _client.User().FindAsync(x => x.Id == id).CAF()).FirstOrDefaultAsync().CAF();

            using var scope = _serviceScopeFactory.CreateScope();
            var mapper = scope.ServiceProvider.GetService<IMapper>();
            
            mapper.Map(settingsViewModel, user);

            await _client.User().FindOneAndReplaceAsync(x => x.Id == user.Id, user).CAF();
        }

        public async Task UpdatePassword(string id, string oldPassword, string newPassword)
        {
            var user = await (await _client.User().FindAsync(x => x.Id == id).CAF()).FirstOrDefaultAsync().CAF();

            if (user == null) return;

            if (!_userFactory.EqualPasswords(oldPassword, user.PasswordHash)) return;

            user.PasswordHash = _userFactory.HashPassword(newPassword);

            await _client.User().FindOneAndReplaceAsync(x => x.Id == user.Id, user).CAF();
        }

        public async Task SetImage(string id, string imageUrl)
        {
            await _client.User().FindOneAndUpdateAsync(x => x.Id == id,
                Builders<User>.Update.Set(x => x.ImageUrl, imageUrl)).CAF();
        }

        public async Task ChangeRole(string id, string role)
        {
            var user = await (await _client.User().FindAsync(x => x.Id == id).CAF()).FirstOrDefaultAsync().CAF();
            
            // Downgrade from editor to basic
            if (role == UserRoles.BasicRole) _userFactory.TakeRole(user, UserRoles.EditorRole);
            else if (role == UserRoles.EditorRole) _userFactory.AddRole(user, UserRoles.EditorRole);

            await _client.User().FindOneAndReplaceAsync(x => x.Id == id, user).CAF();
        }
    }
}