using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atheer.Utilities.Config.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace Atheer.Controllers.Articles
{
    [ApiController]
    public class ArticlesJsonFeedController : ControllerBase
    {
        [HttpGet("/feed.json")]
        public async Task<IActionResult> Get([FromServices] IOptions<Site> siteConfig)
        {
            // TODO support the whole standard with pagination as well
            string baseUrl = Request.Host.ToString();
            
            var jsonFeed = new JsonFeed
            {
                Title = siteConfig.Value.Title,
                UserComment = siteConfig.Value.Description,
                FeedUrl = $"{baseUrl}/feed.json",
                HomePageUrl = baseUrl,
                Items = new List<JsonFeedItem>()
            };

            return Ok(jsonFeed);
        }
    }
}