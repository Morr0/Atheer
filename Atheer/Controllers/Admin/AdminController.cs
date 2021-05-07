using System.Threading.Tasks;
using Atheer.Controllers.Admin.Models;
using Atheer.Extensions;
using Atheer.Services.NavItemsService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Admin
{
    [Route("Admin")]
    [Authorize(Roles = UserRoles.AdminRole)]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult AdminPage()
        {
            string adminUserId = this.GetViewerUserId();
            _logger.LogInformation("Admin: {UserId} viewing Admin page", adminUserId);
            
            return View("AdminPage");
        }

        [HttpGet("Navbar")]
        public IActionResult NavbarItemsPage([FromServices] INavItemsService navItemsService)
        {
            var navItems = navItemsService.Get();
            
            string adminUserId = this.GetViewerUserId();
            _logger.LogInformation("Admin: {UserId} viewing Nav items page", adminUserId);
            
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

            await navItemsService.Add(form.Name, form.Url).CAF();
            
            string adminUserId = this.GetViewerUserId();
            _logger.LogInformation("Admin: {UserId} added a new nav item", adminUserId);

            return RedirectToAction("NavbarItemsPage");
        }

        [HttpPost("Navbar/Remove")]
        public async Task<IActionResult> RemoveNavItem([FromServices] INavItemsService navItemsService, [FromForm] NavItemRemove form)
        {
            string adminUserId = this.GetViewerUserId();
            _logger.LogInformation("Admin: {UserId} trying to delete nav item: {NavItemId}",
                adminUserId, form.Id.ToString());
            
            await navItemsService.Remove(form.Id).ConfigureAwait(false);
            
            _logger.LogInformation("Admin: {UserId} successfully delete nav item: {NavItemId}",
                adminUserId, form.Id.ToString());
            
            return RedirectToAction("NavbarItemsPage");
        }
    }
}