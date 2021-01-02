using System.Threading.Tasks;
using Atheer.Controllers.Dtos;
using Atheer.Models;
using Atheer.Services;
using Atheer.Services.BlogService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    // Used for both editing/adding new posts
    [Route("Article/Edit")]
    public class ArticleEditController : Controller
    {
        private ILogger<ArticleEditController> _logger;
        private IBlogPostService _service;
        private readonly IMapper _mapper;

        public ArticleEditController(ILogger<ArticleEditController> logger, IBlogPostService service
        , IMapper mapper)
        {
            _logger = logger;
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] BlogPostPrimaryKey key)
        {
            BlogPost post = null;
            if (IsNewPost(key.TitleShrinked))
            {
                post = new BlogPost();
            }
            else
            {
                post = await _service.GetSpecific(key).ConfigureAwait(false);
                if (post is null) return Redirect("/");
            }

            var dto = _mapper.Map<BlogPostEditDto>(post);
            return View("ArticleEdit", dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string button, [FromForm] BlogPostEditDto postDto)
        {
            var key = new BlogPostPrimaryKey(postDto.CreatedYear, postDto.TitleShrinked);
            switch (button)
            {
                case "Checkout":
                    return await Checkout(postDto).ConfigureAwait(false);
                case "Page":
                    return VisitPage(ref key);
                case "Delete":
                    return await Delete(key).ConfigureAwait(false);
                default:
                    return Redirect("/");
            }
        }
        
        private async Task<IActionResult> Checkout(BlogPostEditDto postDto)
        {
            if (!ModelState.IsValid) return View("ArticleEdit", postDto);
            
            // ADD
            if (IsNewPost(postDto.TitleShrinked))
            {
                _logger.LogInformation("New");
                await _service.AddPost(postDto).ConfigureAwait(false);
                return RedirectToAction("Index", "Article", new BlogPostPrimaryKey
                {
                    CreatedYear = postDto.CreatedYear,
                    TitleShrinked = postDto.TitleShrinked
                });
            }
            
            // UPDATE
            await _service.Update(postDto).ConfigureAwait(false);
            TempData["Info"] = "Updated post successfully";
            return RedirectToAction("Index", "ArticleEdit", new BlogPostPrimaryKey
            {
                CreatedYear = postDto.CreatedYear,
                TitleShrinked = postDto.TitleShrinked
            });
            
        }

        private IActionResult VisitPage(ref BlogPostPrimaryKey key)
        {
            return RedirectToAction("Index", "Article", key);
        }

        private async Task<IActionResult> Delete(BlogPostPrimaryKey key)
        {
            if (await _service.GetSpecific(key).ConfigureAwait(false) is null) return Redirect("/");
            
            await _service.Delete(key).ConfigureAwait(false);
            return Redirect("/");
        }

        private bool IsNewPost(string titleShrinked)
        {
            return string.IsNullOrEmpty(titleShrinked);
        }
    }
}