using System.Threading.Tasks;
using Atheer.Controllers.Queries;
using Atheer.Controllers.ViewModels;
using Atheer.Exceptions;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using Atheer.Services.UsersService.Exceptions;
using Atheer.Utilities.Config.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Atheer.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, IMapper mapper, ILogger<UserController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> UserView([FromRoute] string userId, [FromServices] IArticleService articleService, 
            [FromQuery] ArticlesQuery query)
        {
            var user = await _userService.Get(userId).ConfigureAwait(false);
            if (user is null) return NotFound();

            // TODO fix user pagination
            var articlesResponse =
                await articleService.Get(ArticlesController.PageSize, 0, viewerUserId: userId, specificUserId: userId).ConfigureAwait(false);

            var viewModel = new UserPageViewModel
            {
                Articles = articlesResponse,
                User = user
            };
            return View("UserPage", viewModel);
        }

        [HttpGet("Settings/{userId}")]
        [Authorize]
        public async Task<IActionResult> UserSettingsView([FromRoute] string userId)
        {
            string viewingUserId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            if (viewingUserId != userId) return RedirectToAction("UserView", userId);

            var user = await _userService.Get(userId).ConfigureAwait(false);
            if (user is null) return Redirect("/");

            var userSettingsVm = _mapper.Map<UserSettingsViewModel>(user);
            return View("UserSettings", userSettingsVm);
        }

        [HttpPost("Settings/{userId}")]
        [Authorize]
        public async Task<IActionResult> UserSettingsPost([FromRoute] string userId,
            [FromForm] UserSettingsUpdate userSettingsUpdate)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UserSettingsView", "User", new {userId});
            }
            
            await _userService.Update(userId, userSettingsUpdate).ConfigureAwait(false);
            return RedirectToAction("UserView", "User", new {userId});
        }

        [HttpGet("ChangePassword/{userId}")]
        [Authorize]
        public IActionResult ChangePasswordView([FromRoute] string userId)
        {
            string viewingUserId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            if (viewingUserId != userId) return Unauthorized();

            return View("ChangePassword", userId);
        }

        [HttpPost("ChangePassword/{userId}")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordPost([FromRoute] string userId,
            [FromForm] UserChangePassword userChangePassword, [FromForm] string button)
        {
            if (button != "change") return RedirectToAction("UserView", new {userId});
            
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ChangePasswordView", new {userId});
            }
            
            string viewingUserId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            if (viewingUserId != userId) return Unauthorized();
            
            // TODO Let user know confirmed new password does not match
            if (userChangePassword.NewPassword != userChangePassword.NewPasswordSecondTime)
            {
                return RedirectToAction("ChangePasswordView", new {userId});
            }
            
            await _userService.UpdatePassword(userId, userChangePassword.OldPassword, userChangePassword.NewPassword).ConfigureAwait(false);
            return RedirectToAction("UserView", new {userId});
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
                string userId = await _userService.Add(registerView).ConfigureAwait(false);
                
                TempData["Info"] = "Successfully registered, please login now";
                return Redirect($"/login?EmailOrUsername={userId}");
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
        }
    }
}