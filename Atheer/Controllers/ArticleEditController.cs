using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
using Atheer.Services;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    // Used for both editing/adding new articles
    [Authorize(Roles = UserRoles.EditorRole)]
    [Route("Article/Edit")]
    public class ArticleEditController : Controller
    {
        private ILogger<ArticleEditController> _logger;
        private IArticleService _service;
        private readonly IMapper _mapper;

        public ArticleEditController(ILogger<ArticleEditController> logger, IArticleService service
        , IMapper mapper)
        {
            _logger = logger;
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ArticlePrimaryKey key)
        {
            Article article = null;
            if (IsNewArticle(key.TitleShrinked))
            {
                article = new Article();
            }
            else
            {
                string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
                
                var vm = await _service.Get(key, userId).ConfigureAwait(false);
                if (vm is null) return Redirect("/");
                
                article = vm.Article;

                if (User.FindFirst(AuthenticationController.CookieUserId)?.Value != article.AuthorId)
                {
                    if (!User.IsInRole(UserRoles.AdminRole)) return Forbid();
                }
            }

            var dto = _mapper.Map<ArticleEditViewModel>(article);
            return View("ArticleEdit", dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string button, [FromForm] ArticleEditViewModel articleViewModel)
        {
            var key = new ArticlePrimaryKey(articleViewModel.CreatedYear, articleViewModel.TitleShrinked);
            switch (button)
            {
                case "Checkout":
                    return await Checkout(key, articleViewModel).ConfigureAwait(false);
                case "Page":
                    return VisitPage(ref key);
                case "Delete":
                    return await Delete(key).ConfigureAwait(false);
                default:
                    return Redirect("/");
            }
        }
        
        private async Task<IActionResult> Checkout(ArticlePrimaryKey key, ArticleEditViewModel articleViewModel)
        {
            // TODO handle FailedOperationException
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            if (!ModelState.IsValid) return View("ArticleEdit", articleViewModel);
            
            // ADD
            if (IsNewArticle(articleViewModel.TitleShrinked))
            {
                key = new ArticlePrimaryKey(articleViewModel.CreatedYear, articleViewModel.TitleShrinked);
                await _service.Add(articleViewModel, userId).ConfigureAwait(false);
                return RedirectToAction("Index", "Article", new ArticlePrimaryKey(
                    articleViewModel.CreatedYear, articleViewModel.TitleShrinked));
            }

            if (!(await _service.AuthorizedFor(key, userId).ConfigureAwait(false)))
            {
                if (!User.IsInRole(UserRoles.AdminRole)) return Forbid();
            }
            
            // UPDATE
            await _service.Update(articleViewModel).ConfigureAwait(false);
            TempData["Info"] = "Updated article successfully";
            return RedirectToAction("Index", "ArticleEdit", key);
        }

        private IActionResult VisitPage(ref ArticlePrimaryKey key)
        {
            return RedirectToAction("Index", "Article", key);
        }

        private async Task<IActionResult> Delete(ArticlePrimaryKey key)
        {
            // TODO handle FailedOperationException
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            if (!(await _service.AuthorizedFor(key, userId).ConfigureAwait(false)))
            {
                if (!User.IsInRole(UserRoles.AdminRole)) return Forbid();
            }
            
            await _service.Delete(key).ConfigureAwait(false);
            return Redirect("/");
        }

        private bool IsNewArticle(string titleShrinked)
        {
            return string.IsNullOrEmpty(titleShrinked);
        }
    }
}