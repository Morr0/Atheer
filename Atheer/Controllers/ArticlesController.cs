using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.Queries;
using Atheer.Controllers.ViewModels;
using Atheer.Services.ArticlesService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    [Route("")]
    [Route("Articles")]
    public class ArticlesController : Controller
    {
        private static int _pageSize = 3;
        
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticleService _service;

        public ArticlesController(ILogger<ArticlesController> logger, IArticleService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IActionResult> Index([FromQuery] ArticlesQuery query)
        {
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;

            var blogResponse = await _service.Get(_pageSize, query.Page, query.Year, query.Tag, userId);
            if (!blogResponse.Articles.Any()) return Redirect("/");
            
            return View("Articles", blogResponse);
        }
    }
}