using System.Threading.Tasks;
using Atheer.Services;
using Atheer.Services.ArticlesService;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers
{
    [Route("api/article")]
    [ApiController]
    public class ArticleApiController : ControllerBase
    {
        private readonly IArticleService _service;

        public ArticleApiController(IArticleService service)
        {
            _service = service;
        }
        
        [HttpPost("like")]
        public async Task<IActionResult> Like([FromQuery] ArticlePrimaryKey key)
        {
            var article = await _service.GetSpecific(key).ConfigureAwait(false);
            if (article is null) return BadRequest();

            await _service.Like(key).ConfigureAwait(false);
            return Ok();
        }
        
        [HttpPost("share")]
        public async Task<IActionResult> Share([FromQuery] ArticlePrimaryKey key)
        {
            var article = await _service.GetSpecific(key).ConfigureAwait(false);
            if (article is null) return BadRequest();

            await _service.Share(key).ConfigureAwait(false);
            return Ok();
        }
    }
}