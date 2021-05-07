using System.Threading.Tasks;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
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
            
            var article = await _service.Get(key, viewerUserId).CAF();
            if (article is null)
            {
                _logger.LogInformation("Asked for article that doesn't exist with key: {CreatedYear}-{TitleShrinked}",
                    key.CreatedYear.ToString(), key.TitleShrinked);
                return Redirect("/");
            }

            if (article.Article.ForceFullyUnlisted && string.IsNullOrEmpty(viewerUserId)) return NotFound();
            
            return View("Article", article);
        }
    }
}