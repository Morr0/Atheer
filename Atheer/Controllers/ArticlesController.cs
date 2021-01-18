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
        private readonly IBlogPostService _service;

        public ArticlesController(ILogger<ArticlesController> logger, IBlogPostService service)
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
            if (blogResponse?.Posts is null) return Redirect("/");

            var viewModel = new ArticlesViewModel
            {
                Posts = blogResponse.Posts,
                SpecificYear = specificYear,
                Year = query.CreatedYear
            };
            return View("Articles", viewModel);
        }
    }
}