using System.IO;
using System.Threading.Tasks;
using Atheer.Controllers.Authentication;
using Atheer.Controllers.User.Models;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Services.FileService;
using Atheer.Services.RecaptchaService;
using Atheer.Services.UsersService;
using Atheer.Services.UsersService.Exceptions;
using Atheer.Utilities.Config.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Atheer.Controllers.User
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
        public async Task<IActionResult> UserView([FromRoute] string userId)
        {
            var user = await _userService.Get(userId).ConfigureAwait(false);
            if (user is null) return NotFound();

            var viewModel = new UserPageViewModel
            {
                User = user
            };
            return View("UserPage", viewModel);
        }

        [HttpGet("Settings/{userId}")]
        [Authorize]
        public async Task<IActionResult> UserSettingsView([FromRoute] string userId)
        {
            string viewingUserId = this.GetViewerUserId();
            if (viewingUserId != userId) return RedirectToAction("UserView", userId);

            var user = await _userService.Get(userId).ConfigureAwait(false);
            if (user is null) return Redirect("/");
            
            _logger.LogInformation("User: {UserId} is accessing user settings page", viewingUserId);

            var userSettingsVm = _mapper.Map<UserSettingsViewModel>(user);
            return View("UserSettings", userSettingsVm);
        }

        [HttpPost("ChangeImage")]
        [Authorize]
        public async Task<IActionResult> ChangeImage([FromForm] UserChangeImage form, [FromServices] IServiceScopeFactory serviceScopeFactory)
        {
            if (!ModelState.IsValid) return Redirect("/");
            
            string viewerUserId = this.GetViewerUserId();
            if (form.UserId != viewerUserId) return Redirect("/");

            string imageUrl = string.Empty;
            await using (var memoryStream = new MemoryStream())
            {
                await form.File.CopyToAsync(memoryStream).ConfigureAwait(false);

                using var scope = serviceScopeFactory.CreateScope();
                var fileService = scope.ServiceProvider.GetService<IFileService>();

                imageUrl = await fileService.Add(FileUse.UserImage, form.UserId, form.File.ContentType, memoryStream)
                    .ConfigureAwait(false);
            }
            
            await _userService.SetImage(form.UserId, imageUrl);
            
            _logger.LogInformation("User: {UserId} has changed their image", viewerUserId);

            return RedirectToAction("UserView", new {userId = form.UserId});
        }

        [HttpPost("RemoveImage")]
        [Authorize]
        public async Task<IActionResult> RemoveImage([FromForm] UserRemoveImage form, [FromServices] IServiceScopeFactory serviceScopeFactory)
        {
            string viewerUserId = this.GetViewerUserId();

            if (form.UserId != viewerUserId && !User.IsInRole(UserRoles.AdminRole)) return Redirect("/");

            await _userService.SetImage(form.UserId, string.Empty).ConfigureAwait(false);
            
            using var scope = serviceScopeFactory.CreateScope();
            var fileService = scope.ServiceProvider.GetService<IFileService>();

            await fileService.Remove(FileUse.UserImage, form.UserId).ConfigureAwait(false);

            // For S3 to update their clusters
            await Task.Delay(1000).ConfigureAwait(false);
            
            _logger.LogInformation("User: {UserId} has removed their image", viewerUserId);

            return RedirectToAction("UserView", new {userId = form.UserId});
        }

        [HttpPost("Admin/ChangeRole")]
        [Authorize(Roles = UserRoles.AdminRole)]
        public async Task<IActionResult> AdminChangeRoleOfUser([FromForm] ChangeRoleByAdmin form)
        {
            if (!ModelState.IsValid) return Redirect("/");
            
            string adminUserId = this.GetViewerUserId();
            // Don't allow an admin to play with self roles
            if (form.UserId == adminUserId) return Redirect("/");
            
            await _userService.ChangeRole(form.UserId, form.NewRole).ConfigureAwait(false);
            
            _logger.LogInformation("Admin: {AdminId} has changed roles of user: {UserId}", adminUserId, form.UserId);

            return RedirectToAction("UserView", new {userId = form.UserId});
        }

        [HttpPost("Settings/{userId}")]
        [Authorize]
        public async Task<IActionResult> UserSettingsPost([FromRoute] string userId,
            [FromForm] UserSettingsUpdate userSettingsUpdate)
        {
            string viewingUserId = this.GetViewerUserId();
            if (viewingUserId != userId) return Unauthorized();
            
            if (!ModelState.IsValid)
            {
                return View("UserSettings", new UserSettingsViewModel
                {
                    Id = userId,
                    Bio = userSettingsUpdate.Bio,
                    DateCreated = userSettingsUpdate.DateCreated,
                    Name = userSettingsUpdate.Name
                });
            }
            
            await _userService.Update(userId, userSettingsUpdate).CAF();
            
            _logger.LogInformation("User: {UserId} has updated user settings", viewingUserId);
            
            return RedirectToAction("UserView", "User", new {userId});
        }

        [HttpGet("ChangePassword/{userId}")]
        [Authorize]
        public IActionResult ChangePasswordView([FromRoute] string userId)
        {
            string viewingUserId = this.GetViewerUserId();
            if (viewingUserId != userId) return Unauthorized();
            if (User.HasClaim(AuthenticationController.CookieOAuthUser, "true")) return Unauthorized();
            
            _logger.LogInformation("User: {UserId} is accessing change password page", viewingUserId);

            return View("ChangePassword");
        }

        [HttpPost("ChangePassword/{userId}")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordPost([FromRoute] string userId,
            [FromForm] UserChangePassword userChangePassword, [FromForm] string button)
        {
            if (button != "change") return RedirectToAction("UserView", new {userId});
            if (!ModelState.IsValid) return View("ChangePassword");
            
            string viewingUserId = this.GetViewerUserId();
            if (viewingUserId != userId) return Unauthorized();

            await _userService.UpdatePassword(userId, userChangePassword.OldPassword, userChangePassword.NewPassword).ConfigureAwait(false);
            
            _logger.LogInformation("User: {UserId} has changed their password", viewingUserId);
            
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
        public async Task<IActionResult> Register([FromForm] RegisterViewModel registerView, [FromServices] IServiceScopeFactory serviceScopeFactory)
        {
            if (!ModelState.IsValid) return View("Register", registerView);
            
            _logger.LogInformation("Attempt to register new user");
            
            if (User.Identity?.IsAuthenticated == true) return Redirect("/");

            if (!(await ProceedAfterHandlingRecaptchaIfAppropriate(serviceScopeFactory, registerView.RecaptchaResponse).CAF()))
            {
                await Task.Delay(2000).CAF();
                _logger.LogInformation("Failed reCaptcha on user registeration");

                return View("Register", registerView);
            }

            try
            {
                string userId = await _userService.Add(registerView).CAF();
                
                _logger.LogInformation("Successfully registered new user: {UserId}", userId);
                
                TempData["Info"] = "Successfully registered, please login now";
                return Redirect($"/login?EmailOrUsername={userId}");
            }
            catch (FailedOperationException)
            {
                return View("Register", registerView);
            }
            catch (UserWithThisEmailAlreadyExistsException)
            {
                ViewData["EmailsExistsError"] = "Email registered already";
                return View("Register", registerView);
            }
        }

        private async Task<bool> ProceedAfterHandlingRecaptchaIfAppropriate(IServiceScopeFactory serviceScopeFactory, string recaptchaUserResponse)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var recaptchaConfig = scope.ServiceProvider.GetService<IOptions<Recaptcha>>();

            if (!recaptchaConfig.Value.Enabled) return true;

            var recaptchaService = scope.ServiceProvider.GetService<IRecaptchaService>();
            return await recaptchaService.IsValidClient(recaptchaUserResponse).ConfigureAwait(false);
        }
    }
}