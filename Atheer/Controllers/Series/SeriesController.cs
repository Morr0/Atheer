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

namespace Atheer.Controllers.Series
{
    [Authorize(Roles = UserRoles.EditorRole)]
    [Route("Series")]
    public class SeriesController : Controller
    {
        private readonly IArticleService _articleService;

        public SeriesController(IArticleService articleService)
        {
            _articleService = articleService;
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

            return RedirectToAction("Index");
        }
        
        [HttpPost("Finish/{id:int}")]
        public async Task<IActionResult> FinishSeries([FromRoute] int id)
        {
            string viewerUserId = this.GetViewerUserId();
            await _articleService.FinishArticleSeries(viewerUserId, id).CAF();

            return RedirectToAction("Index");
        }
    }
}