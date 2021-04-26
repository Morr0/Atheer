using System.Text.Json.Serialization;

namespace Atheer.Controllers.Article
{
    // https://jsonfeed.org/version/1.1
    public class JsonFeedItem
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("date_published")] public string DatePublished { get; set; }
        [JsonPropertyName("date_modified")] public string DateModified { get; set; }
        [JsonPropertyName("summary")] public string Summary { get; set; }
        [JsonPropertyName("content_html")] public string ContentHtml { get; set; }
    }
}