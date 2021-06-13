using System;
using System.Threading.Tasks;
using Atheer.Controllers.Article.Queries;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
using Atheer.Services.TagService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Article
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
        public async Task<IActionResult> Index([FromQuery] ArticlesQuery query, [FromRoute] int page = 0)
        {
            page = Math.Max(0, page);

            string viewerUserId = this.GetViewerUserId();

            ArticlesResponse blogResponse = null;
            if (string.IsNullOrEmpty(query.Q))
            {
                blogResponse = await _articleService.Get(PageSize, page, query.Year, query.Tag, specificUserId: query.UserId,
                        viewerUserId: viewerUserId, oldest: query.Oldest).CAF();
            }
            // else
            // {
            //     _logger.LogInformation("Searching for {SearchQuery}", query.Q);
            //     // To minimize spam of searches
            //     await Task.Delay(1000).CAF();
            //     
            //     blogResponse = await _articleService.Get(PageSize, query.Q).CAF();
            // }
            
            if (blogResponse is null) return Redirect("/");
            if (blogResponse.Articles.Count == 0 && blogResponse.CurrentPage > 0) return Redirect("/");
            
            blogResponse.MostPopularTags = await _tagService.GetTopTags(10).ConfigureAwait(false);

            return View("Articles", blogResponse);
        }
    }
}