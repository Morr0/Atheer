using System.Threading.Tasks;
using Atheer.Models;
using Atheer.Services;
using Atheer.Services.BlogService;
using Atheer.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    [Route("Article/{CreatedYear}/{TitleShrinked}")]
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IBlogPostService _service;

        public ArticleController(ILogger<ArticleController> logger, IBlogPostService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] BlogPostPrimaryKey route)
        {
            var post = await _service.GetSpecific(route).ConfigureAwait(false);
            
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            if (!post.HasAccessTo(userId, isAdmin: User.IsInRole(UserRoles.AdminRole))) return Forbid();
            
            if (post is null) return Redirect("/");

            return View("Article", post);
        }
    }
}