﻿using System.Threading.Tasks;
using Atheer.Models;
using Atheer.Services;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    [Route("Article/{CreatedYear}/{TitleShrinked}")]
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService _service;

        public ArticleController(ILogger<ArticleController> logger, IArticleService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] ArticlePrimaryKey route)
        {
            var article = await _service.GetSpecific(route).ConfigureAwait(false);
            if (article is null) return Redirect("/");
            
            string userId = User.FindFirst(AuthenticationController.CookieUserId)?.Value;
            if (!article.Article.HasAccessTo(userId, isAdmin: User.IsInRole(UserRoles.AdminRole))) return Forbid();

            return View("Article", article);
        }
    }
}