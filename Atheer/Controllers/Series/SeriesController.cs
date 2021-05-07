using System;
using System.Threading.Tasks;
using Atheer.Controllers.Series.Queries;
using Atheer.Controllers.Series.Requests;
using Atheer.Controllers.Series.ViewModels;
using Atheer.Extensions;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Series
{
    // TODO recheck authorizations
    [Authorize(Roles = UserRoles.EditorRole)]
    [Route("Series")]
    public class SeriesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<SeriesController> _logger;

        public SeriesController(IArticleService articleService, ILogger<SeriesController> logger)
        {
            _articleService = articleService;
            _logger = logger;
        }
        
        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] SeriesQuery query)
        {
            Enum.TryParse(query.SeriesType, true, out ArticleSeriesType seriesType);
            ViewData["SeriesType"] = seriesType;

            string userId = this.GetViewerUserId();
            var series = await _articleService.GetSeries(userId, seriesType).CAF();
            
            return View("ManySeries", new ManySeriesViewModel
            {
                Series = series
            });
        }

        [HttpGet("Add")]
        public IActionResult AddSeriesView()
        {
            return View("AddSeries");
        }
        
        [HttpPost("Add")]
        public async Task<IActionResult> AddSeries([FromForm] AddSeriesRequest request)
        {
            if (!ModelState.IsValid) return View("AddSeries", request);

            string viewerUserId = this.GetViewerUserId();
            await _articleService.AddSeries(viewerUserId, request).CAF();
            
            _logger.LogInformation("User: {UserId} added a new series with title: {Title}",
                viewerUserId, request.Title);

            return RedirectToAction("Index");
        }
        
        [HttpPost("Finish/{id:int}")]
        public async Task<IActionResult> FinishSeries([FromRoute] int id)
        {
            string viewerUserId = this.GetViewerUserId();
            await _articleService.FinishArticleSeries(viewerUserId, id).CAF();
            
            _logger.LogInformation("User: {UserId} finished series with id: {SeriesId}",
                viewerUserId, id.ToString());

            return RedirectToAction("Index");
        }

        [HttpGet("{seriesId:int}")]
        public async Task<IActionResult> SingleSeriesView([FromRoute] int seriesId)
        {
            var series = await _articleService.GetSeries(seriesId).CAF();
            return View("SingleSeries", new SingleSeriesViewModel
            {
                Series = series
            });
        }
    }
}