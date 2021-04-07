using System.Threading.Tasks;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Article
{
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService _service;

        public ArticleController(ILogger<ArticleController> logger, IArticleService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("Article/{CreatedYear}/{TitleShrinked}")]
        public async Task<IActionResult> Index([FromRoute] ArticlePrimaryKey key)
        {
            string viewerUserId = this.GetViewerUserId();
            
            var article = await _service.Get(key, viewerUserId).ConfigureAwait(false);
            if (article is null) return Redirect("/");
            
            return View("Article", article);
        }
        
        [Authorize(Roles = UserRoles.EditorRole)]
        [HttpGet("Article/Series")]
        public async Task<IActionResult> SeriesView()
        {
            return View("Series");
        }
    }
}