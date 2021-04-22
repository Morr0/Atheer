using System;
using System.Threading.Tasks;
using Atheer.Controllers.Article.Requests;
using Atheer.Controllers.Utilities.Filters;
using Atheer.Exceptions;
using Atheer.Services.ArticlesService;
using Atheer.Services.FileService;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers.Article
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
            string cdnUrl = fileService.GetCdnUrlFromFileKey(request.S3BucketKey);
            var key = new ArticlePrimaryKey(request.CreatedYear, request.TitleShrinked);
            await _service.CompletedNarration(key, cdnUrl).ConfigureAwait(false);
            
            return Ok();
        }
    }
}