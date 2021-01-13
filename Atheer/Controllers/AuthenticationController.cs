using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
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
        [HttpGet("Login")]
        public IActionResult LoginView()
        {
            return View("Login");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginView)
        {
            var user = await _userService.GetFromEmail(loginView.Email).ConfigureAwait(false);
            if (user is not null)
            {
                string sessionId = _sessionsService.Login(loginView, user);
                if (sessionId is not null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                        , ClaimsPrincipal(ref sessionId, ref user)).ConfigureAwait(false);

                    return Redirect("/");
                }
            }

            TempData["Info"] = "Inputted email/password is not correct";
            return RedirectToAction("LoginView");
        }

        private ClaimsPrincipal ClaimsPrincipal(ref string sessionId, ref User user)
        {
            var claims = new List<Claim>
            {
                new Claim(CookieSessionId, sessionId),
                new Claim(ClaimTypes.Role, user.Roles)
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }
        
        [Authorize]
        [HttpGet("Logout")]
        public IActionResult LogoutView()
        {
            return View("Logout");
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutPost()
        {
            var t = this;
            string sessionId = Request.HttpContext.User.FindFirst(CookieSessionId)?.Value;

            _logger.LogInformation($"{HttpContext.User.Claims.Count()} dgsjg");
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