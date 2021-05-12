using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Atheer.Controllers.Authentication.Models;
using Atheer.Controllers.Authentication.Requests;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Services.OAuthService;
using Atheer.Services.UsersService;
using Atheer.Services.UsersService.Exceptions;
using Atheer.Services.UsersService.Models;
using Atheer.Utilities.Config.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Atheer.Controllers.Authentication
{
    public class AuthenticationController : Controller
    {
        public static string CookieUserId = "userId";
        public static readonly string CookieOAuthUser = "OAuth";
        
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUserService userService, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        
        [HttpGet("Login")]
        [ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult LoginView([FromQuery] string emailOrUsername)
        {
            if (User.Identity?.IsAuthenticated == true) return Redirect("/");

            return View("Login", new LoginViewModel
            {
                EmailOrUsername =  emailOrUsername,
                AttemptsLeft = UserService.AttemptsUntilFreeze
            });
        }

        [HttpPost("Login")]
        [ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Login([FromForm] LoginRequest request, [FromQuery] string returnUrl = "/")
        {
            // To minimize spam
            await Task.Delay(1000).CAF();
            
            if (!ModelState.IsValid) return View("Login", new LoginViewModel
            {
                EmailOrUsername = request.EmailOrUsername,
                AttemptsLeft = UserService.AttemptsUntilFreeze
            });
            
            _logger.LogInformation("Trying to login for EmailOrUsername: {user}", request.EmailOrUsername);

            UserLoginAttemptResponse userLoginAttempt;
            try
            {
                userLoginAttempt = await _userService.TryLogin(request.EmailOrUsername, request.Password).CAF();
            }
            catch (OAuthUserCannotLoginUsingPasswordException)
            {
                _logger.LogInformation("Denied login to EmailOrUsername: {user} due to it being an OAuth user",
                    request.EmailOrUsername);
                
                // Intentional 10 seconds, because only an attacker tries that
                await Task.Delay(10000).CAF();
                return RedirectToAction("Login");
            }

            switch (userLoginAttempt.AttemptStatus)
            {
                case UserLoginAttemptStatus.InvalidCredentials:
                    _logger.LogInformation("Denied login to EmailOrUsername: {user} due to incorrect credentials",
                        request.EmailOrUsername);
                
                    TempData["Info"] = "Inputted email/password is not correct";
                    return View("Login", new LoginViewModel
                    {
                        EmailOrUsername = request.EmailOrUsername,
                        AttemptsLeft = userLoginAttempt.LoginAttemptsLeft
                    });
                
                case UserLoginAttemptStatus.ExceededAttempts:
                    // i.e. now FROZEN
                    _logger.LogInformation("Denied login to user id: {user} due to exceeding allowed attempts for this window", 
                        userLoginAttempt.User.Id);

                    await Task.Delay(1000).CAF();

                    return View("LoginFreeze", new LoginFreezeViewModel(request.EmailOrUsername, 
                        userLoginAttempt.NextLoginAttemptTime.Value));
                
                case UserLoginAttemptStatus.LoggedIn:
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                        , ClaimsPrincipal(userLoginAttempt.User.Id, userLoginAttempt.User.Roles)).CAF();

                    if (!Url.IsLocalUrl(returnUrl)) return Redirect("/");
                
                    _logger.LogInformation("Successfully logged in for EmailOrUsername: {user}", request.EmailOrUsername);

                    return LocalRedirect(returnUrl);
                
                default: return Redirect("/");
            }

            // TODO look at reseting login attempts after a successful login/logout within short timeframe maybe?
            // TODO handle the case of alreaday frozen
            // TODO add freeze cookie for frozen attempts and cache response
        }

        private ClaimsPrincipal ClaimsPrincipal(string userId, string roles, bool oAuthUser = false)
        {
            var claims = new List<Claim>
            {
                new Claim(CookieUserId, userId)
            };
            
            foreach (var role in roles.Split(','))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (oAuthUser)
            {
                claims.Add(new Claim(CookieOAuthUser, "true"));
            }
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }
        
        [Authorize]
        [HttpGet("Logout")]
        [ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).CAF();
            string loggedInUserId = this.GetViewerUserId();
            _logger.LogInformation("User id: {userId} successfully logged out", loggedInUserId);

            return Redirect("/");
        }

        [HttpGet("/Auth/Callback/Github")]
        [ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> GithubRedirect([FromQuery] GithubOAuthRedirectQuery query, [FromServices] IOAuthService oAuthService, 
            [FromServices] IOptions<GithubOAuth> githubOauthConfig)
        {
            if (User.Identity?.IsAuthenticated == true) return Redirect("/");
            if (!githubOauthConfig.Value.Enabled) return Redirect("/");
            if (string.IsNullOrEmpty(query.Code)) return Redirect("/");

            OAuthUserInfo userInfo;
            try
            {
                userInfo = await oAuthService.GetUserInfo(OAuthProvider.Github, query.Code).CAF();
            }
            catch (FailedOperationException)
            {
                _logger.LogWarning("Bounced OAuth attempt from OAuth provider: {Provider}", OAuthProvider.Github.ToString());
                return Redirect("/");
            }
            (string userId, string roles) = await _userService.AddOrUpdateOAuthUser(userInfo).CAF();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                , ClaimsPrincipal(userId, roles, oAuthUser: true)).CAF();
            await _userService.SetLogin(userId).CAF();
            
            _logger.LogInformation("Successfully logged in for User id: {UserId} from OAuth Provider: {Provider}", 
                userId, OAuthProvider.Github.ToString());
            
            return Redirect("/");
        }
        
    }
}