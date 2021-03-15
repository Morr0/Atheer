using System;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.ArticleEdit.Models;
using Atheer.Controllers.Authentication;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services.ArticlesService;
using Atheer.Services.TagService;
using Atheer.Services.UsersService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.ArticleEdit
{
    // Used for both editing/adding new articles
    [Route("Article/Edit")]
    public class ArticleEditController : Controller
    {
        private ILogger<ArticleEditController> _logger;
        private IArticleService _articleService;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ArticleEditController(ILogger<ArticleEditController> logger, IArticleService articleService
        , IMapper mapper, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _articleService = articleService;
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ArticlePrimaryKey key)
        {
            string viewerUserId = this.GetViewerUserId();
            
            Atheer.Models.Article article = null;
            string tagsAsString = string.Empty;
            bool isNewArticle = IsNewArticle(key.TitleShrinked);
            
            if (isNewArticle)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var userService = scope.ServiceProvider.GetService<IUserService>();
                if (!(await userService.HasRole(viewerUserId, UserRoles.EditorRole).ConfigureAwait(false))) return Redirect("/");
                
                article = new Atheer.Models.Article();
            }
            else
            {
                string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
                
                var vm = await _articleService.Get(key, userId).ConfigureAwait(false);
                if (vm is null) return Redirect("/");
                
                if (viewerUserId != vm.Article.AuthorId)
                {
                    if (!User.IsInRole(UserRoles.AdminRole)) return Forbid();
                }
                
                article = vm.Article;
                tagsAsString = string.Join(", ", vm.Tags.Select(x => x.Title));
            }

            var dto = _mapper.Map<ArticleEditViewModel>(article);
            dto.Schedule = DateTimeExtensions.GetDateOnly(article.ScheduledSinceDate);
            dto.TagsAsString = tagsAsString;
            
            return View("ArticleEdit", dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string button, [FromForm] ArticleEditViewModel articleViewModel, 
            [FromForm] ArticleEditChangeAuthorByAdmin changeAuthorByAdmin, [FromForm] ArticleEditTags tags)
        {
            var key = new ArticlePrimaryKey(articleViewModel.CreatedYear, articleViewModel.TitleShrinked);
            switch (button)
            {
                case "Checkout":
                    return await Checkout(key, articleViewModel, changeAuthorByAdmin, tags).ConfigureAwait(false);
                case "Page":
                    return VisitPage(ref key);
                case "Delete":
                    return await Delete(key).ConfigureAwait(false);
                default:
                    return Redirect("/");
            }
        }

        private async Task<IActionResult> Checkout(ArticlePrimaryKey key, ArticleEditViewModel articleViewModel, 
            ArticleEditChangeAuthorByAdmin authorChangeByAdmin, ArticleEditTags tags)
        {
            // TODO handle FailedOperationException
            string viewerUserId = this.GetViewerUserId();
            if (!ModelState.IsValid)
            {
                return View("ArticleEdit", articleViewModel);
            }
            
            using var scope = _serviceScopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            bool newArticle = IsNewArticle(articleViewModel.TitleShrinked);
            if (newArticle)
            {
                // ADD
                if (!(await userService.HasRole(viewerUserId, UserRoles.EditorRole).ConfigureAwait(false)))
                    return Redirect("/");
                
                key = await _articleService.Add(articleViewModel, viewerUserId).ConfigureAwait(false);
            }
            else
            {
                // UPDATE
                if (!(await AuthorizedFor(key, viewerUserId).ConfigureAwait(false))) return Forbid();

                await ChangeAuthorIfChangedByAdmin(userService, articleViewModel, authorChangeByAdmin).ConfigureAwait(false);

                await _articleService.Update(articleViewModel).ConfigureAwait(false);
                TempData["Info"] = "Updated article successfully";
            }
            
            var tagService = scope.ServiceProvider.GetRequiredService<ITagService>();
            var individualTags = tags.TagsAsString.Split(',').Where(x => x != ",");
            await tagService.AddOrUpdateTagsPerArticle(key, individualTags).ConfigureAwait(false);

            return newArticle ? RedirectToAction("Index", "Article", key) : RedirectToAction("Index", "ArticleEdit", key);
        }

        private async Task<bool> AuthorizedFor(ArticlePrimaryKey key, string viewerUserId)
        {
            if (!(await _articleService.AuthorizedFor(key, viewerUserId).ConfigureAwait(false)))
            {
                if (!User.IsInRole(UserRoles.AdminRole)) return false;
            }

            return true;
        }

        private async Task ChangeAuthorIfChangedByAdmin(IUserService userService, ArticleEditViewModel articleViewModel,
            ArticleEditChangeAuthorByAdmin changeAuthorByAdmin)
        {
            if (string.IsNullOrEmpty(articleViewModel.AuthorId) || !User.IsInRole(UserRoles.AdminRole)) return;
            if (!changeAuthorByAdmin.IsValid()) return;
            if (articleViewModel.AuthorId == changeAuthorByAdmin.NewAuthorId) return;

            if (await userService.Exists(changeAuthorByAdmin.NewAuthorId).ConfigureAwait(false))
            {
                articleViewModel.AuthorId = changeAuthorByAdmin.NewAuthorId;
            }
            else
            {
                TempData["Err"] = "Author id was not updated due to selected user non-existent";
            }
        }

        private IActionResult VisitPage(ref ArticlePrimaryKey key)
        {
            return RedirectToAction("Index", "Article", key);
        }

        private async Task<IActionResult> Delete(ArticlePrimaryKey key)
        {
            string viewerUserId = this.GetViewerUserId();
            if (!(await AuthorizedFor(key, viewerUserId).ConfigureAwait(false))) return Forbid();

            try
            {
                await _articleService.Delete(key).ConfigureAwait(false);
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