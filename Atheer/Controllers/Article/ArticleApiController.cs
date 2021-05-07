using System.Threading.Tasks;
using Atheer.Controllers.Article.Requests;
using Atheer.Controllers.Utilities.Filters;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
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
                await _service.Like(key).ConfigureAwait(false);
                return Ok();
            }
            catch (IncorrectOperationException)
            {
                return BadRequest();
            }
        }
        
        [HttpPost("share")]
        public async Task<IActionResult> Share([FromQuery] ArticlePrimaryKey key)
        {
            try
            {
                await _service.Share(key).ConfigureAwait(false);
                return Ok();
            }
            catch (IncorrectOperationException)
            {
                return BadRequest();
            }
        }

        [RestrictToInternalNetwork]
        [HttpPatch("narration/complete")]
        public async Task<IActionResult> CompleteNarrationWebhook([FromBody] CompletedArticleNarrationRequest request, 
            [FromServices] IFileService fileService)
        {
            _logger.LogInformation("Received narration complete for article key: {CreatedYear}-{TitleShrinked}",
                request.CreatedYear.ToString(), request.TitleShrinked);
            
            string cdnUrl = fileService.GetCdnUrlFromFileKey(request.S3BucketKey);
            var key = new ArticlePrimaryKey(request.CreatedYear, request.TitleShrinked);
            await _service.CompletedNarration(key, cdnUrl).CAF();
            
            _logger.LogInformation("Completed narration article key: {CreatedYear}-{TitleShrinked}",
                request.CreatedYear.ToString(), request.TitleShrinked);
            
            return Ok();
        }
    }
}