// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Atheer.Controllers.Article.Queries;
// using Atheer.Services.ArticlesService;
// using Atheer.Utilities.Config.Models;
// using AutoMapper;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
//
// namespace Atheer.Controllers.Article
// {
//     [ApiController]
//     [Route("json.feed")]
//     public class ArticlesJsonFeedController : ControllerBase
//     {
//         private readonly ILogger<ArticlesJsonFeedController> _logger;
//
//         public ArticlesJsonFeedController(ILogger<ArticlesJsonFeedController> logger)
//         {
//             _logger = logger;
//         }
//         
//         // TODO attach narration url if narratable
//         // TODO ability to subscribe to hubs
//         
//         [Produces("application/feed+json", "application/json")]
//         [ResponseCache(Duration = 900, Location = ResponseCacheLocation.Any)]
//         [HttpGet]
//         public async Task<IActionResult> Get([FromServices] IOptions<Site> siteConfig, [FromServices] IArticleService articleService, 
//             [FromServices] IMapper mapper, [FromQuery] JsonFeedPageQuery query)
//         {
//             string baseUrl = $"{Request.Scheme}://{Request.Host.ToString()}";
//
//             var articlesResponse = await articleService.Get(ArticlesController.PageSize, query.Page);
//             var jsonFeedItems = mapper.Map<List<JsonFeedItem>>(articlesResponse.Articles);
//             string icon = string.IsNullOrEmpty(siteConfig.Value.IconUrl) ? $"{baseUrl}/favicon.ico"
//                 : siteConfig.Value.IconUrl;
//
//             var jsonFeed = new JsonFeed
//             {
//                 Title = siteConfig.Value.Title,
//                 Description = siteConfig.Value.Description,
//                 FeedUrl = $"{baseUrl}/feed.json?page={query.Page.ToString()}",
//                 NextUrl = articlesResponse.AnyNext ? $"{baseUrl}/feed.json?page={(query.Page + 1).ToString()}" : null,
//                 HomePageUrl = baseUrl,
//                 Items = jsonFeedItems,
//                 Favicon = icon,
//                 Icon = icon
//             };
//             
//             _logger.LogInformation("Served JSON Feed on page {Page}", query.Page.ToString());
//
//             return Ok(jsonFeed);
//         }
//     }
// }