using System;
using System.Threading.Tasks;
using Atheer.Controllers.Articles.Queries;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
using Atheer.Services.TagService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Articles
{
    [Route("")]
    [Route("Articles")]
    public class ArticlesController : Controller
    {
        public static readonly int PageSize = 10;
        
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticleService _articleService;
        private readonly ITagService _tagService;

        public ArticlesController(ILogger<ArticlesController> logger, IArticleService articleService, ITagService tagService)
        {
            _logger = logger;
            _articleService = articleService;
            _tagService = tagService;
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
                blogResponse = await _articleService.Get(PageSize, page, query.Year, query.Tag, viewerUserId, oldest: query.Oldest)
                    .ConfigureAwait(false);
            }
            else
            {
                blogResponse = await _articleService.Get(PageSize, searchQuery.Q);
            }
            
            if (blogResponse is null) return Redirect("/");

            blogResponse.MostPopularTags = await _tagService.GetTopTags(10).ConfigureAwait(false);

            return View("Articles", blogResponse);
        }
    }
}