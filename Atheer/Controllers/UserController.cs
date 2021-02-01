﻿using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Exceptions;
using Atheer.Services.UsersService;
using Atheer.Services.UsersService.Exceptions;
using Atheer.Utilities.Config.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Atheer.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult UserView([FromRoute] string userId)
        {
            return Redirect("/");
        }
        
        [HttpGet("/Register")]
        public IActionResult RegisterView(RegisterViewModel? registerViewModel, 
            [FromServices] IOptionsSnapshot<Site> options)
        {
            if (!options.Value.CanRegister) return Redirect("NotFound");
            if (User.Identity?.IsAuthenticated == true) return Redirect("/");
            
            return View("Register", registerViewModel);
        }
        
        [HttpPost("/Register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel registerView)
        {
            if (User.Identity?.IsAuthenticated == true) return Redirect("/");

            try
            {
                throw new FailedOperationException();
                await _userService.Add(registerView).ConfigureAwait(false);
            }
            catch (FailedOperationException)
            {
                return View("Register", registerView);
            }
            catch (UserWithThisEmailAlreadyExistsException)
            {
                TempData["EmailsExistsError"] = "Email registered already";
                return RedirectToAction("RegisterView", registerView);
            }
            
            TempData["Info"] = "Successfully registered, please login now";
            return Redirect("/login");
        }
    }
}