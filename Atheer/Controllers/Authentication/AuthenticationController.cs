﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Atheer.Controllers.Authentication.Models;
using Atheer.Services.UserSessionsService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Authentication
{
    public class AuthenticationController : Controller
    {
        private const string CookieSessionId = "sessionId";
        public static string CookieUserId = "userId";
        
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
            if (!ModelState.IsValid) return View("Login", loginView);
            
            var user = await _userService.GetFromEmailOrUsername(loginView.EmailOrUsername).ConfigureAwait(false);
            if (user is not null)
            {
                string sessionId = _sessionsService.Login(loginView, user);
                if (sessionId is not null)
                {
                    await _userService.SetLogin(user.Id).ConfigureAwait(false);
                    
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                        , ClaimsPrincipal(ref sessionId, ref user)).ConfigureAwait(false);

                    if (!Url.IsLocalUrl(returnUrl)) return Redirect("/");
                    
                    return LocalRedirect(returnUrl);
                }
            }

            TempData["Info"] = "Inputted email/password is not correct";
            return RedirectToAction("LoginView");
        }

        private ClaimsPrincipal ClaimsPrincipal(ref string sessionId, ref Atheer.Models.User user)
        {
            var claims = new List<Claim>
            {
                new Claim(CookieSessionId, sessionId),
                new Claim(CookieUserId, user.Id),
                new Claim(ClaimTypes.Name, user.FullName())
            };
            
            foreach (var role in user.Roles.Split(','))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }
        
        [Authorize]
        [HttpGet("Logout")]
        [ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult LogoutView()
        {
            return View("Logout");
        }

        [Authorize]
        [HttpPost("Logout")]
        [ResponseCache(Duration = 0, NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> LogoutPost()
        {
            string sessionId = Request.HttpContext.User.FindFirst(CookieSessionId)?.Value;

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