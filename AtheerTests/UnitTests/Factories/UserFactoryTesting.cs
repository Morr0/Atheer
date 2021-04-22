using System;
using System.Linq;
using Atheer.Controllers.User.Models;
using Atheer.Models;
using Atheer.Services.OAuthService;
using Atheer.Services.UsersService;
using Atheer.Services.Utilities.TimeService;
using Atheer.Utilities;
using AutoMapper;
using Xunit;

namespace AtheerTests.UnitTests.Factories
{
    public class UserFactoryTesting
    {
        private IMapper _mapper = new Mapper(new MapperConfiguration(expression => expression.AddProfile(typeof(ModelMappingsProfile))));
        private readonly UserFactory _factory;

        public UserFactoryTesting()
        {
            _factory = new UserFactory(_mapper, new TimeService());
        }

        [Fact]
        public void ShouldCreateUserWithNormalUserRegisteration()
        {
            var registerationModel = new RegisterViewModel
            {
                Name = "Dave Jo",
                Email = "dave@ramihikmat.net",
                Password = "12345678",
                Bio = string.Empty
            };

            var user = _factory.Create(registerationModel);
            
            Assert.Equal(registerationModel.Name, user.Name);
            Assert.Equal(registerationModel.Email, user.Email);
            Assert.NotEqual(registerationModel.Password, user.PasswordHash);
            Assert.Equal(registerationModel.Bio, user.Bio);
            Assert.False(user.OAuthUser);
            Assert.Equal(UserRoles.BasicRole, user.Roles);
        }

        [Fact]
        public void ShouldCreateUserWithOAuth()
        {
            var oAuthUserInfo = new OAuthUserInfo
            {
                Email = "rami@ramihikmat.net",
                Name = "J",
                OAuthProvider = "SomeOAuth",
                OAuthUsername = "joko",
                OAuthProviderId = "123"
            };

            var user = _factory.Create(oAuthUserInfo);
            
            Assert.Equal(oAuthUserInfo.Name, user.Name);
            Assert.Equal(oAuthUserInfo.Email, user.Email);
            Assert.True(user.OAuthUser);
            Assert.Equal(oAuthUserInfo.OAuthProvider, user.OAuthProvider);
            Assert.True(string.IsNullOrEmpty(user.PasswordHash));
            Assert.Equal(UserRoles.BasicRole, user.Roles);
        }

        [Fact]
        public void ShouldCreateIdOutOfEmail()
        {
            string email = "rami@ramihikmat.net";
            string expectedId = "rami";

            string actualId = _factory.Id(email);
            
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public void ShouldCreateOAuthLogicalIdBasedOnProvider()
        {
            string provider = "O";
            string providerId = "123";
            string expectedLogicalId = $"{provider}-{providerId}";

            string actualLogicalId = _factory.OAuthLogicalId(provider, providerId);

            Assert.Equal(expectedLogicalId, actualLogicalId);
        }

        [Fact]
        public void ShouldAddRoleGivenItExists()
        {
            var user = new User
            {
                Roles = UserRoles.BasicRole
            };
            string role = UserRoles.EditorRole;
            
            _factory.AddRole(user, role);

            Assert.Contains(UserRoles.BasicRole, user.Roles);
            Assert.Contains(role, user.Roles);
        }

        [Fact]
        public void ShouldNotBeAbleToAddBasicRoleAsItExistsAlready()
        {
            var user = new User
            {
                Roles = UserRoles.BasicRole
            };
            
            _factory.AddRole(user, UserRoles.BasicRole);

            Assert.True(user.Roles.Split(',').Count() == 1);
            Assert.Contains(UserRoles.BasicRole, user.Roles);
        }

        [Fact]
        public void ShouldTakeRoleAwayGivenItIsNotBasicRole()
        {
            var user = new User
            {
                Roles = $"{UserRoles.BasicRole},{UserRoles.EditorRole}"
            };
            
            _factory.TakeRole(user, UserRoles.EditorRole);
            
            Assert.DoesNotContain(UserRoles.EditorRole, user.Roles);
            Assert.Contains(UserRoles.BasicRole, user.Roles);
        }

        [Fact]
        public void ShouldNotBeAbleToTakeBasicRoleAway()
        {
            var user = new User
            {
                Roles = UserRoles.BasicRole
            };
            
            _factory.TakeRole(user, UserRoles.BasicRole);
            
            Assert.Contains(UserRoles.BasicRole, user.Roles);
        }
    }
}