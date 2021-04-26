using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atheer.Controllers.Article.Queries;
using Atheer.Services.ArticlesService;
using Atheer.Utilities.Config.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Atheer.Controllers.Article
{
    [ApiController]
    [Route("json.feed")]
    public class ArticlesJsonFeedController : ControllerBase
    {
        // TODO attach narration url if narratable
        // TODO attach icon of site
        // TODO ability to subscribe to hubs
        
        [Produces("application/feed+json", "application/json")]
        [ResponseCache(Duration = 900, Location = ResponseCacheLocation.Any)]
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IOptions<Site> siteConfig, [FromServices] IArticleService articleService, 
            [FromServices] IMapper mapper, [FromQuery] JsonFeedPageQuery query)
        {
            string baseUrl = Request.Host.ToString();

            var articlesResponse = await articleService.Get(ArticlesController.PageSize, query.Page);
            var jsonFeedItems = mapper.Map<List<JsonFeedItem>>(articlesResponse.Articles);

            var jsonFeed = new JsonFeed
            {
                Title = siteConfig.Value.Title,
                Description = siteConfig.Value.Description,
                FeedUrl = $"{baseUrl}/feed.json?page={query.Page.ToString()}",
                NextUrl = articlesResponse.AnyNext ? $"{baseUrl}/feed.json?page={(query.Page + 1).ToString()}" : null,
                HomePageUrl = baseUrl,
                Items = jsonFeedItems,
                Favicon = null,
                Icon = null
            };

            return Ok(jsonFeed);
        }
    }
}