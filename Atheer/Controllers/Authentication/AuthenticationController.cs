using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Atheer.Controllers.Authentication.Models;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Services.OAuthService;
using Atheer.Services.UserSessionsService;
using Atheer.Services.UsersService;
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
        private const string CookieSessionId = "sessionId";
        public static string CookieUserId = "userId";
        public static readonly string CookieOAuthUser = "OAuth";
        
        private readonly IUserService _userService;
        private readonly IUserSessionsService _sessionsService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUserService userService, IUserSessionsService sessionsService, 
            ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _sessionsService = sessionsService;
            _logger = logger;
        }
        
        [HttpGet("Login")]
        [ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult LoginView([FromQuery] string emailOrUsername)
        {
            if (User.Identity?.IsAuthenticated == true) return Redirect("/");

            return View("Login", new LoginViewModel
            {
                EmailOrUsername =  emailOrUsername
            });
        }

        [HttpPost("Login")]
        [ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginView, [FromQuery] string returnUrl = "/")
        {
            await Task.Delay(1000).CAF();
            
            if (!ModelState.IsValid) return View("Login", loginView);
            
            var user = await _userService.GetFromEmailOrUsernameForLogin(loginView.EmailOrUsername).ConfigureAwait(false);
            if (user is not null)
            {
                string sessionId = _sessionsService.Login(loginView, user);
                if (sessionId is not null)
                {
                    await _userService.SetLogin(user.Id).ConfigureAwait(false);
                    
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                        , ClaimsPrincipal(sessionId, user.Id, user.Roles)).ConfigureAwait(false);

                    if (!Url.IsLocalUrl(returnUrl)) return Redirect("/");
                    
                    return LocalRedirect(returnUrl);
                }
            }

            TempData["Info"] = "Inputted email/password is not correct";
            return RedirectToAction("LoginView");
        }

        private ClaimsPrincipal ClaimsPrincipal(string sessionId, string userId, string roles, bool oAuthUser = false)
        {
            var claims = new List<Claim>
            {
                new Claim(CookieSessionId, sessionId),
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
                userInfo = await oAuthService.GetUserInfo(OAuthProvider.Github, query.Code).ConfigureAwait(false);
            }
            catch (FailedOperationException)
            {
                return Redirect("/");
            }
            (string userId, string roles) = await _userService.AddOrUpdateOAuthUser(userInfo).ConfigureAwait(false);

            string sessionId = _sessionsService.Login(userId);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                , ClaimsPrincipal(sessionId, userId, roles, oAuthUser: true)).ConfigureAwait(false);
            await _userService.SetLogin(userId).ConfigureAwait(false);
            
            return Redirect("/");
        }
        
    }
}