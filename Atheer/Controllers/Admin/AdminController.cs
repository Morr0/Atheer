using System.Threading.Tasks;
using Atheer.Controllers.Admin.Models;
using Atheer.Services.NavItemsService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers.Admin
{
    [Route("Admin")]
    [Authorize(Roles = UserRoles.AdminRole)]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult AdminPage()
        {
            return View("AdminPage");
        }

        [HttpGet("Navbar")]
        public IActionResult NavbarItemsPage([FromServices] INavItemsService navItemsService)
        {
            var navItems = navItemsService.Get();
            return View("NavbarItemsPage", navItems);
        }

        [HttpPost("Navbar/Add")]
        public async Task<IActionResult> AddNavItem([FromServices] INavItemsService navItemsService, [FromForm] NavItemAdd form)
        {
            if (!ModelState.IsValid)
            {
                TempData["Model"] = "Please provide the Name and Url";
                return RedirectToAction("NavbarItemsPage");
            }

            await navItemsService.Add(form.Name, form.Url).ConfigureAwait(false);

            return RedirectToAction("NavbarItemsPage");
        }

        [HttpPost("Navbar/Remove")]
        public async Task<IActionResult> RemoveNavItem([FromServices] INavItemsService navItemsService, [FromForm] NavItemRemove form)
        {
            await navItemsService.Remove(form.Id).ConfigureAwait(false);
            
            return RedirectToAction("NavbarItemsPage");
        }
    }
}