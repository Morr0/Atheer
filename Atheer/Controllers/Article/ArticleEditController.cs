using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.Article.Models;
using Atheer.Controllers.Article.Requests;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
using Atheer.Services.ArticlesService.Exceptions;
using Atheer.Services.TagService;
using Atheer.Services.UsersService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers.Article
{
    [Route("Article")]
    public class ArticleEditController : Controller
    {
        private IArticleService _articleService;
        private readonly IMapper _mapper;

        public ArticleEditController(IArticleService articleService, IMapper mapper)
        {
            _articleService = articleService;
            _mapper = mapper;
        }

        private async Task<bool> AuthorizedFor(ArticlePrimaryKey key, string viewerUserId)
        {
            return await _articleService.AuthorizedFor(key, viewerUserId).ConfigureAwait(false) || 
                   User.IsInRole(UserRoles.AdminRole);
        }

        [HttpGet("Add")]
        [Authorize(Roles = UserRoles.EditorRole)]
        public async Task<IActionResult> AddArticleView()
        {
            string userId = this.GetViewerUserId();

            ViewBag.Series = await _articleService.GetSeriesFor(userId).CAF();
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
            try
            {
                bool authorizedFor = await AuthorizedFor(key, userId).CAF();
                if (!authorizedFor) return Forbid();
            }
            catch (ArticleNotFoundException)
            {
                return NotFound();
            }

            var articleVm = await _articleService.Get(key, userId).CAF();
            if (articleVm is null) return NotFound();
                
            var vm = _mapper.Map<UpdateArticleViewModel>(articleVm.Article);
            vm.TagsAsString = ITagService.TagsToString(articleVm.Tags);

            var series = await _articleService.GetSeriesFor(userId).CAF();
            // A finished series will not be included in the list above
            bool currSeriesIsFinished = articleVm.Article.SeriesId is not null && 
                                        series.All(x => x.Id != articleVm.Article.SeriesId);
            if (currSeriesIsFinished)
            {
                ViewBag.CurrentSeries = await _articleService.GetSeries(articleVm.Article.SeriesId).CAF();
            }

            ViewBag.Series = series;
            ViewBag.Key = key;
            return View("UpdateArticle", vm);
        }

        [HttpPost("Update/{CreatedYear:int}/{TitleShrinked}")]
        public async ValueTask<IActionResult> UpdateArticlePost([FromRoute] ArticlePrimaryKey key,
            [FromForm] UpdateArticleViewModel viewModel, [FromServices] ITagService tagService)
        {
            if (!ModelState.IsValid) return View("UpdateArticle", viewModel);
            
            string userId = this.GetViewerUserId();
            try
            {
                bool authorizedFor = await AuthorizedFor(key, userId).CAF();
                if (!authorizedFor) return Forbid();
            }
            catch (ArticleNotFoundException)
            {
                return NotFound();
            }
            
            await _articleService.Update(userId, key, viewModel).CAF();
            await tagService.AddOrUpdateTagsPerArticle(key, viewModel.TagsAsString).CAF();

            return RedirectToAction("UpdateArticleView", key);
        }
    }
}