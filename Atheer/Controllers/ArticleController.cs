using System.Threading.Tasks;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    [Route("Article/{CreatedYear}/{TitleShrinked}")]
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService _service;

        public ArticleController(ILogger<ArticleController> logger, IArticleService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] ArticlePrimaryKey key)
        {
            string viewerUserId = this.GetViewerUserId();
            
            var article = await _service.Get(key, viewerUserId).ConfigureAwait(false);
            if (article is null) return Redirect("/");
            
            return View("Article", article);
        }
    }
}