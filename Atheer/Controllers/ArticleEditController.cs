using System.Threading.Tasks;
using Atheer.Controllers.Dtos;
using Atheer.Controllers.Queries;
using AtheerBackend.DTOs;
using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Models;
using AtheerBackend.Services.BlogService;
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
        public async Task<IActionResult> Index([FromQuery] BlogPostEditQuery editQuery)
        {
            BlogPost post = null;
            if (IsNewPost(editQuery.TitleShrinked))
            {
                post = new BlogPost();
            }
            else
            {
                post =
                    await _service.GetSpecific(new BlogPostPrimaryKey(editQuery.CreatedYear, editQuery.TitleShrinked))
                        .ConfigureAwait(false);
                if (post is null) return Redirect("/");
            }

            return View("ArticleEdit", post);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromForm] BlogPostEditDto postDto)
        {
            BlogPost post = null;
            if (IsNewPost(postDto.TitleShrinked))
            {
                post = await _service.AddPost(postDto).ConfigureAwait(false);
                return LocalRedirect($"/article/{post.CreatedYear}/{post.TitleShrinked}");
                // return RedirectToAction("Index", "Article", post);
            }
            else
            {
                post = await _service.UpdatePost(postDto).ConfigureAwait(false);
                // Refresh as GET
                return RedirectToAction("Index", post);
            }
        }

        private bool IsNewPost(string titleShrinked)
        {
            return string.IsNullOrEmpty(titleShrinked);
        }
    }
}