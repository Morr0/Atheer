using System.Threading.Tasks;
using Atheer.Controllers.Queries;
using Atheer.Controllers.ViewModels;
using Atheer.Exceptions;
using Atheer.Services.ArticlesService;
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

        [HttpGet("{userId}")]
        public async Task<IActionResult> UserView([FromRoute] string userId, [FromServices] IArticleService articleService, 
            [FromQuery] ArticlesQuery query)
        {
            var user = await _userService.Get(userId).ConfigureAwait(false);
            if (user is null) return NotFound();

            var articlesResponse =
                await articleService.Get(ArticlesController.PageSize, query.Page, viewerUserId: userId, specificUserId: userId).ConfigureAwait(false);

            var viewModel = new UserPageViewModel
            {
                Articles = articlesResponse,
                User = user
            };
            return View("UserPage", viewModel);
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