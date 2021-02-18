using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ArticleEditController(ILogger<ArticleEditController> logger, IArticleService service
        , IMapper mapper, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _service = service;
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ArticlePrimaryKey key)
        {
            bool isAdmin = User.IsInRole(UserRoles.AdminRole);
            Article article = null;
            string tagsAsString = "";
            bool isNewArticle = IsNewArticle(key.TitleShrinked);
            
            if (isNewArticle)
            {
                article = new Article();
            }
            else
            {
                string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
                
                var vm = await _service.Get(key, userId).ConfigureAwait(false);
                if (vm is null) return Redirect("/");
                
                article = vm.Article;

                tagsAsString = Tag.TagsAsString(vm.Tags);
                
                if (User.FindFirst(AuthenticationController.CookieUserId)?.Value != article.AuthorId)
                {
                    if (!isAdmin) return Forbid();
                }
            }

            var dto = _mapper.Map<ArticleEditViewModel>(article);
            dto.TagsAsString = tagsAsString;
            dto.Schedule = DateTimeExtensions.GetDateOnly(article.ScheduledSinceDate);
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
            _logger.LogInformation("CALL");
            // TODO handle FailedOperationException
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Errrors");
                foreach (var pair in ModelState)
                {
                    _logger.LogInformation($"{pair.Key}: ");
                    foreach (var error in pair.Value.Errors)
                    {
                        _logger.LogInformation(error.ErrorMessage);
                    }
                }
                // TODO fix this to redirect
                return View("ArticleEdit", articleViewModel);
            }

            _logger.LogInformation("CALL2");
            articleViewModel.TagsAsString = articleViewModel.TagsAsString.TrimEnd();

            // ADD
            if (IsNewArticle(articleViewModel.TitleShrinked))
            {
                key = new ArticlePrimaryKey(articleViewModel.CreatedYear, articleViewModel.TitleShrinked);
                await _service.Add(articleViewModel, userId).ConfigureAwait(false);
                return RedirectToAction("Index", "Article", new ArticlePrimaryKey(
                    articleViewModel.CreatedYear, articleViewModel.TitleShrinked));
            }

            // UPDATE
            if (!(await _service.AuthorizedFor(key, userId).ConfigureAwait(false)))
            {
                if (!User.IsInRole(UserRoles.AdminRole)) return Forbid();
            }
            // Check if updating user that it exists
            // If user non-existent will update everything except for the author id
            if (articleViewModel.AuthorId != articleViewModel.NewAuthorId)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var userService = scope.ServiceProvider.GetService<IUserService>();
                
                if (!(await userService.Exists(articleViewModel.NewAuthorId).ConfigureAwait(false)))
                {
                    articleViewModel.NewAuthorId = articleViewModel.AuthorId;
                    if (User.IsInRole(UserRoles.AdminRole))
                        TempData["Err"] = "Author id was not updated due to selected user non-existent";
                }
                else
                {
                    articleViewModel.AuthorId = articleViewModel.NewAuthorId;
                }
            }
            
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
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            if (!(await _service.AuthorizedFor(key, userId).ConfigureAwait(false)))
            {
                if (!User.IsInRole(UserRoles.AdminRole)) return Forbid();
            }

            try
            {
                await _service.Delete(key).ConfigureAwait(false);
                return Redirect("/");
            }
            catch (FailedOperationException)
            {
                return RedirectToAction("Index", key);
            }
            
        }

        private bool IsNewArticle(string titleShrinked)
        {
            return string.IsNullOrEmpty(titleShrinked);
        }
    }
}