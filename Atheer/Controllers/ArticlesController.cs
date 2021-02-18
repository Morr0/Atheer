using System;
using System.ComponentModel.DataAnnotations;
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
        public static readonly int PageSize = 10;
        
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticleService _service;

        public ArticlesController(ILogger<ArticlesController> logger, IArticleService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("{page}")]
        public async Task<IActionResult> Index([FromQuery] ArticlesQuery query, [FromRoute] int page = 0)
        {
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            page = Math.Max(0, page);

            var blogResponse = await _service.Get(PageSize, page, query.Year, query.Tag, userId).ConfigureAwait(false);
            
            if (blogResponse is null) return Redirect("/");
            if (!blogResponse.Articles.Any()) return Redirect("/");
            
            return View("Articles", blogResponse);
        }
    }
}