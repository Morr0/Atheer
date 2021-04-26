using System.Text.Json.Serialization;

namespace Atheer.Controllers.Article
{
    // https://jsonfeed.org/version/1.1
    public class JsonFeedItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        [JsonPropertyName("date_published")] public string DatePublished { get; set; }
        public string Summary { get; set; }
        [JsonPropertyName("content_text")] public string ContentMarkdown { get; set; }
        // public string Authors { get; set; }
        // public string Tags { get; set; }
    }
}