using System.Threading.Tasks;
using Atheer.Controllers.Queries;
using Atheer.Controllers.ViewModels;
using Atheer.Services.BlogService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    [Route("")]
    [Route("Articles")]
    public class ArticlesController : Controller
    {
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticleService _service;

        public ArticlesController(ILogger<ArticlesController> logger, IArticleService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IActionResult> Index([FromQuery] YearQuery query)
        {
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;

            bool specificYear = query.CreatedYear != 0;
            var blogResponse = specificYear ?
                    await _service.GetByYear(query.CreatedYear, 500, userId: userId).ConfigureAwait(false)
                :   await _service.Get(500, userId: userId).ConfigureAwait(false);
            if (blogResponse?.Articles is null) return Redirect("/");

            var viewModel = new ArticlesViewModel
            {
                Articles = blogResponse.Articles,
                SpecificYear = specificYear,
                Year = query.CreatedYear
            };
            return View("Articles", viewModel);
        }
    }
}