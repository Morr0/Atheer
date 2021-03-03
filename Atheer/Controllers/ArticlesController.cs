using System;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.Queries;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    [Route("")]
    [Route("Articles")]
    public class ArticlesController : Controller
    {
        public static readonly int PageSize = 10;
        
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticleService _service;

        public ArticlesController(ILogger<ArticlesController> logger, IArticleService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [HttpGet("{page}")]
        public async Task<IActionResult> Index([FromQuery] ArticlesQuery query, [FromQuery] ArticlesSearchQuery searchQuery,
            [FromRoute] int page = 0, string userId = null)
        {
            page = Math.Max(0, page);
            if (!string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("UserView", "User", new
                {
                    userId,
                    page
                });
            }

            string viewerUserId = this.GetViewerUserId();

            ArticlesResponse blogResponse = null;
            if (string.IsNullOrEmpty(searchQuery.Q))
            {
                blogResponse = await _service.Get(PageSize, page, query.Year, query.Tag, viewerUserId, oldest: query.Oldest)
                    .ConfigureAwait(false);
            }
            else
            {
                blogResponse = await _service.Get(PageSize, searchQuery.Q);
            }

            if (blogResponse is null) return Redirect("/");
            
            return View("Articles", blogResponse);
        }
    }
}