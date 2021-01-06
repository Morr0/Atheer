using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Atheer.Controllers.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers
{
    public class AuthenticationController : Controller
    {
        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult LoginView()
        {
            return View("Login");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginView)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginView.Username)
            };
            

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                , claimsPrincipal).ConfigureAwait(false);
            return Redirect("/");
        }
        
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            return Redirect("/");
        }

        [AllowAnonymous]
        [HttpGet("register")]
        public IActionResult RegisterView()
        {
            return View("Register");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel registerView)
        {
            // TODO do logic
            TempData["Info"] = "Successfully registered, please login now";
            return Redirect("/login");
        }
    }
}