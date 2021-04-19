using System;
using System.Threading.Tasks;
using Atheer.Controllers.Article.Queries;
using Atheer.Controllers.Article.Requests;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Article
{
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService _service;

        public ArticleController(ILogger<ArticleController> logger, IArticleService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("Article/{CreatedYear}/{TitleShrinked}")]
        public async Task<IActionResult> Index([FromRoute] ArticlePrimaryKey key)
        {
            string viewerUserId = this.GetViewerUserId();
            
            var article = await _service.Get(key, viewerUserId).ConfigureAwait(false);
            if (article is null) return Redirect("/");
            
            return View("Article", article);
        }

        [HttpGet("Article/Series")]
        public async Task<IActionResult> SeriesView([FromQuery] ArticleSeriesQuery query)
        {
            Enum.TryParse(query.SeriesType, true, out ArticleSeriesType seriesType);
            ViewData["SeriesType"] = seriesType.ToString();

            string viewerUserId = this.GetViewerUserId();
            var series = await _service.GetSeries(viewerUserId, seriesType).ConfigureAwait(false);
            
            return View("Series", series);
        }

        [Authorize(Roles = UserRoles.EditorRole)]
        [HttpPost("Article/Series")]
        public async Task<IActionResult> AddSeries([FromForm] AddArticleSeries request)
        {
            string viewerUserId = this.GetViewerUserId();
            if (!ModelState.IsValid)
            {
                var series = await _service.GetSeries(viewerUserId, ArticleSeriesType.All).ConfigureAwait(false);
                return View("Series", series);
            }

            await _service.AddSeries(viewerUserId, request).ConfigureAwait(false);

            return RedirectToAction("SeriesView");
        }

        [Authorize(Roles = UserRoles.EditorRole)]
        [HttpPost("Article/Series/Finish/{id:int}")]
        public async Task<IActionResult> FinishSeries([FromRoute] int id)
        {
            string viewerUserId = this.GetViewerUserId();
            await _service.FinishArticleSeries(viewerUserId, id).ConfigureAwait(false);

            return RedirectToAction("SeriesView");
        }
    }
}