using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Atheer.Controllers.Article
{
    // https://jsonfeed.org/version/1.1
    public class JsonFeed
    {
        [JsonPropertyName("version")] public string Version { get; } = "https://jsonfeed.org/version/1.1";
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("home_page_url")] public string HomePageUrl { get; set; }
        [JsonPropertyName("feed_url")] public string FeedUrl { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("next_url")] public string NextUrl { get; set; }
        [JsonPropertyName("icon")] public string Icon { get; set; }
        [JsonPropertyName("favicon")] public string Favicon { get; set; }
        [JsonPropertyName("items")] public ICollection<JsonFeedItem> Items { get; set; }
    }
}