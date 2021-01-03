using System.Threading.Tasks;
using Atheer.Services;
using Atheer.Services.BlogService;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers
{
    [Route("api/article")]
    [ApiController]
    public class ArticleApiController : ControllerBase
    {
        private readonly IBlogPostService _service;

        public ArticleApiController(IBlogPostService service)
        {
            _service = service;
        }
        
        [HttpPost("like")]
        public async Task<IActionResult> Like([FromQuery] BlogPostPrimaryKey key)
        {
            var post = await _service.GetSpecific(key).ConfigureAwait(false);
            if (post is null) return BadRequest();

            await _service.Like(key).ConfigureAwait(false);
            return Ok();
        }
        
        [HttpPost("share")]
        public async Task<IActionResult> Share([FromQuery] BlogPostPrimaryKey key)
        {
            var post = await _service.GetSpecific(key).ConfigureAwait(false);
            if (post is null) return BadRequest();

            await _service.Share(key).ConfigureAwait(false);
            return Ok();
        }
    }
}