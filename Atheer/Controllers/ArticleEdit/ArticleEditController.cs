using System;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.ArticleEdit.Models;
using Atheer.Controllers.Authentication;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
using Atheer.Services.TagService;
using Atheer.Services.UsersService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        private async Task<bool> AuthorizedFor(ArticlePrimaryKey key, string viewerUserId)
        {
            return await _articleService.AuthorizedFor(key, viewerUserId).ConfigureAwait(false) || 
                   User.IsInRole(UserRoles.AdminRole);
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

        [HttpGet("Add")]
        [Authorize(Roles = UserRoles.EditorRole)]
        public async ValueTask<IActionResult> AddArticleView()
        {
            return View("AddArticle");
        }
        
        [HttpPost("Add")]
        [Authorize(Roles = UserRoles.EditorRole)]
        public async Task<IActionResult> AddArticlePost([FromForm] AddArticleRequest request, [FromServices] ITagService tagService)
        {
            if (!ModelState.IsValid) return View("AddArticle", request);

            string userId = this.GetViewerUserId();
            var key = await _articleService.Add(userId, request).CAF();
            await tagService.AddOrUpdateTagsPerArticle(key, request.TagsAsString).ConfigureAwait(false);

            return RedirectToAction("Index", "Article", key);
        }

        [HttpGet("Update/{CreatedYear:int}/{TitleShrinked}")]
        public async Task<IActionResult> UpdateArticleView([FromRoute] ArticlePrimaryKey key)
        {
            string userId = this.GetViewerUserId();
            bool authorizedFor = await AuthorizedFor(key, userId).CAF();
            if (!authorizedFor) return Forbid();

            var articleVm = await _articleService.Get(key, userId).CAF();
            var vm = _mapper.Map<UpdateArticleViewModel>(articleVm.Article);
            vm.TagsAsString = ITagService.TagsToString(articleVm.Tags);

            return View("UpdateArticle", vm);
        }

        [HttpPost("Update/{CreatedYear:int}/{TitleShrinked}")]
        public async ValueTask<IActionResult> UpdateArticlePost([FromRoute] ArticlePrimaryKey key,
            [FromForm] UpdateArticleViewModel viewModel, [FromServices] ITagService tagService)
        {
            if (!ModelState.IsValid) return View("UpdateArticle", viewModel);
            
            string userId = this.GetViewerUserId();
            bool authorizedFor = await AuthorizedFor(key, userId).CAF();
            if (!authorizedFor) return Forbid();

            await _articleService.Update(userId, key, viewModel).CAF();
            await tagService.AddOrUpdateTagsPerArticle(key, viewModel.TagsAsString).CAF();

            return RedirectToAction("UpdateArticleView", key);
        }
    }
}