using System;
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
using Atheer.Services.UsersService.Models.LoginAttempts;
using Atheer.Utilities.Config.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Atheer.Controllers.Authentication
{
    public class AuthenticationController : Controller
    {
        public static string CookieUserId = "userId";
        public static readonly string CookieOAuthUser = "OAuth";

        private static readonly string FreezeCookieName = "FreezeCookie";
        
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUserService userService, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        
        [HttpGet("Login")]
        public IActionResult LoginView([FromQuery] string username, [FromQuery] bool emphasizeUsername)
        {
            if (User.Identity?.IsAuthenticated == true) return Redirect("/");

            bool freezeCookieExists = Request.Cookies.TryGetValue(FreezeCookieName, out string datetime);
            if (freezeCookieExists)
            {
                return View("LoginFreeze", new LoginFreezeViewModel(DateTime.Parse(datetime)));
            }

            return View("Login", new LoginViewModel
            {
                Username =  username,
                EmphasizeUsername = emphasizeUsername
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
                Username = request.Username
            });
            
            _logger.LogInformation("Trying to login for Username: {user}", request.Username);

            LoginAttemptResponse loginResponse;
            try
            {
                loginResponse = await _userService.TryLogin(request.Username, request.Password).CAF();
            }
            catch (OAuthUserCannotLoginUsingPasswordException)
            {
                _logger.LogWarning("Denied login to Username: {user} due to it being an OAuth user",
                    request.Username);
                
                // Intentional 10 seconds, because only an attacker tries that
                await Task.Delay(10000).CAF();
                return RedirectToAction("Login");
            }
            
            if (loginResponse is IncorrectCredentialsLoginAttemptResponse incorrectCredentialsResponse)
            {
                _logger.LogInformation("Denied login to Username: {user} due to incorrect credentials",
                    request.Username);
                
                return View("Login", new LoginViewModel
                {
                    Username = request.Username,
                    AttemptsLeft = incorrectCredentialsResponse.AttemptsLeft - 1,
                    ShowAttemptsLeft = true
                });
            }
            
            if (loginResponse is FreezeLoginAttemptResponseResponse freezeResponse)
            {
                // i.e. now FROZEN
                _logger.LogInformation("Denied login to Username: {user} due to exceeding allowed attempts for this window", 
                    freezeResponse.User.Id);
                
                Response.Cookies.Append(FreezeCookieName, freezeResponse.Until.GetString(), new CookieOptions
                {
                    HttpOnly = true,
                    MaxAge = freezeResponse.Until - DateTime.UtcNow
                });

                return View("LoginFreeze", new LoginFreezeViewModel(freezeResponse.Until));
            }

            var proceedResponse = loginResponse as ProceedLoginAttemptResponse;
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                , ClaimsPrincipal(proceedResponse.User.Id, proceedResponse.User.Roles)).CAF();

            _logger.LogInformation("Successfully logged in for Username: {user}", request.Username);
            
            if (!Url.IsLocalUrl(returnUrl)) return Redirect("/");
            
            return LocalRedirect(returnUrl);
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
            _logger.LogInformation("Username: {userId} successfully logged out", loggedInUserId);

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
            await _userService.TryLoginOAuth(userId).CAF();
            
            _logger.LogInformation("Successfully logged in for User id: {UserId} from OAuth Provider: {Provider}", 
                userId, OAuthProvider.Github.ToString());
            
            return Redirect("/");
        }
        
    }
}