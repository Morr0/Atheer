using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
using Atheer.Services;
using Atheer.Services.BlogService;
using Atheer.Services.UserService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    // Used for both editing/adding new posts
    [Authorize(Roles = UserRoles.EditorRole)]
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

                if (User.FindFirst(AuthenticationController.CookieUserId)?.Value != post.AuthorId)
                {
                    if (!User.IsInRole(UserRoles.AdminRole)) return Forbid();
                }
            }

            var dto = _mapper.Map<BlogPostEditViewModel>(post);
            return View("ArticleEdit", dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string button, [FromForm] BlogPostEditViewModel postViewModel)
        {
            var key = new BlogPostPrimaryKey(postViewModel.CreatedYear, postViewModel.TitleShrinked);
            switch (button)
            {
                case "Checkout":
                    return await Checkout(key, postViewModel).ConfigureAwait(false);
                case "Page":
                    return VisitPage(ref key);
                case "Delete":
                    return await Delete(key).ConfigureAwait(false);
                default:
                    return Redirect("/");
            }
        }
        
        private async Task<IActionResult> Checkout(BlogPostPrimaryKey key, BlogPostEditViewModel postViewModel)
        {
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;

            if (!ModelState.IsValid) return View("ArticleEdit", postViewModel);
            
            // ADD
            if (IsNewPost(postViewModel.TitleShrinked))
            {
                key = new BlogPostPrimaryKey(postViewModel.CreatedYear, postViewModel.TitleShrinked);
                await _service.AddPost(postViewModel, userId).ConfigureAwait(false);
                return RedirectToAction("Index", "Article", new BlogPostPrimaryKey(
                    postViewModel.CreatedYear, postViewModel.TitleShrinked));
            }

            if (!(await _service.AuthorizedFor(key, userId).ConfigureAwait(false)))
            {
                if (!User.IsInRole(UserRoles.AdminRole)) return Forbid();
            }
            
            // UPDATE
            await _service.Update(postViewModel).ConfigureAwait(false);
            TempData["Info"] = "Updated post successfully";
            return RedirectToAction("Index", "ArticleEdit", key);
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