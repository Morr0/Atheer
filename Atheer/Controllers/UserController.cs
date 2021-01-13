using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Services.UserService;
using Atheer.Services.UserService.Exceptions;
using Microsoft.AspNetCore.Mvc;

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
        
        [HttpGet("/register")]
        public IActionResult RegisterView(RegisterViewModel? registerViewModel)
        {
            return View("Register", registerViewModel);
        }
        
        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel registerView)
        {
            try
            {
                await _userService.Add(registerView).ConfigureAwait(false);
            }
            catch (UserWithThisEmailAlreadyExistsException e)
            {
                TempData["EmailsExistsError"] = "Email registered already";
                return RedirectToAction("RegisterView", registerView);
            }
            
            TempData["Info"] = "Successfully registered, please login now";
            return Redirect("/login");
        }
    }
}