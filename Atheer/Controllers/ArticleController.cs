using System.Threading.Tasks;
using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Models;
using AtheerBackend.Services.BlogService;
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

        [FromRoute]
        public BlogPostPrimaryKey Route { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var post = await _service.GetSpecific(Route).ConfigureAwait(false);
            if (post is null) return Redirect("/");

            return View("Article", post);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string submit, [FromForm] BlogPostPrimaryKey key)
        {
            _logger.LogInformation($"Like: {key.CreatedYear}, {key.TitleShrinked}");
            BlogPost post = null;
            switch (submit)
            {
                case "Like":
                    post = await _service.Like(key).ConfigureAwait(false);
                    break;
                case "Share":
                    post = await _service.Share(key).ConfigureAwait(false);
                    break;
            }
            if (post is null) return Redirect("Error");
            
            return View("Article", post);
        }
    }
}