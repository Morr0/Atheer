using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Atheer.Controllers.Article
{
    // https://jsonfeed.org/version/1.1
    public class JsonFeed
    {
        public string Version { get; } = "https://jsonfeed.org/version/1.1";
        [JsonPropertyName("user_comment")]
        public string UserComment { get; set; }
        public string Title { get; set; }
        [JsonPropertyName("home_page_url")] public string HomePageUrl { get; set; }
        [JsonPropertyName("feed_url")] public string FeedUrl { get; set; }
        public IList<JsonFeedItem> Items { get; set; }
    }
}