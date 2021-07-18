using System.Threading.Tasks;
using Atheer.Controllers.Article.Requests;
using Atheer.Controllers.Utilities.Filters;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
using Atheer.Services.ArticlesService.Exceptions;
using Atheer.Services.FileService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Article
{
    [Route("api/article")]
    [ApiController]
    public class ArticleApiController : ControllerBase
    {
        private readonly IArticleService _service;
        private readonly ILogger<ArticleApiController> _logger;

        public ArticleApiController(IArticleService service, ILogger<ArticleApiController> logger)
        {
            _service = service;
            _logger = logger;
        }
        
        [HttpPost("like")]
        public async Task<IActionResult> Like([FromQuery] ArticlePrimaryKey key)
        {
            try
            {
                await _service.Like(key).CAF();
                return Ok();
            }
            catch (ArticleNotFoundException)
            {
                _logger.LogWarning("Attempt to like a non-existent article: {ArticleKey}", key.ToString());
                return BadRequest();
            }
        }
        
        [HttpPost("share")]
        public async Task<IActionResult> Share([FromQuery] ArticlePrimaryKey key)
        {
            try
            {
                await _service.Share(key).CAF();
                return Ok();
            }
            catch (ArticleNotFoundException)
            {
                _logger.LogWarning("Attempt to share a non-existent article: {ArticleKey}", key.ToString());
                return BadRequest();
            }
        }
    }
}