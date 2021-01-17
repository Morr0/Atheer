using Atheer.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers
{
    [Route("Admin")]
    [Authorize(Roles = UserRoles.AdminRole)]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/");
        }
    }
}