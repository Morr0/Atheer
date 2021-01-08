using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atheer.Controllers.Dtos;
using Atheer.Services.UserService;
using Atheer.Services.UserService.Exceptions;
using Atheer.Services.UserSessionsService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    public class AuthenticationController : Controller
    {
        private const string CookieSessionId = "sessionId";
        
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
        
        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult LoginView()
        {
            return View("Login");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginView)
        {
            var user = await _userService.GetFromEmail(loginView.Email).ConfigureAwait(false);
            if (user is not null)
            {
                string sessionId = _sessionsService.Login(loginView, user);
                if (sessionId is not null)
                {
                    _logger.LogInformation(sessionId);
                    var claims = Claims(ref sessionId, user.Roles);
                    _logger.LogInformation(claims.Count.ToString());
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                        , claimsPrincipal).ConfigureAwait(false);
                    _logger.LogInformation($"Claims count from .User: {HttpContext.User.Claims.Count()}");
                    _logger.LogInformation(HttpContext.User.Identity.IsAuthenticated.ToString());
                    return Redirect("/");
                }
            }

            TempData["Info"] = "Inputted email/password is not correct";
            return RedirectToAction("LoginView");
        }

        private IList<Claim> Claims(ref string sessionId, string roles)
        {
            IList<string> rolesList = roles.Split(',');

            var claims = new List<Claim>
            {
                new Claim(CookieSessionId, sessionId),
            };

            foreach (var role in rolesList)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
        
        [HttpGet("logout")]
        public IActionResult LogoutView()
        {
            return View("Logout");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutPost()
        {
            string sessionId = HttpContext.User.FindFirst(CookieSessionId)?.Value;

            _logger.LogInformation($"{HttpContext.User.Claims.Count()} dgsjg");
            foreach (var claim in HttpContext.User.Claims)
            {
                _logger.LogInformation($"{claim.Type} : {claim.Value} : {claim.ValueType}");
            }
            _logger.LogInformation(sessionId);
            if (_sessionsService.LoggedIn(sessionId))
            {
                _sessionsService.Logout(sessionId);
                // Take the cookie out even if was not actually logged in as below
            }
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            return Redirect("/");
        }
    }
}