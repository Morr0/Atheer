using System.Threading.Tasks;
using AtheerBackend.Services.BlogService;
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

        public async Task<IActionResult> Index()
        {
            var blogResponse = (await _service.Get(500).ConfigureAwait(false));
            if (blogResponse?.Posts is null) return Redirect("/");

            return View("Articles", blogResponse.Posts);
        }
    }
}