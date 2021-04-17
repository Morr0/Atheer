using System.Text.Json.Serialization;

namespace Atheer.Services.ArticlesService.Models
{
    public class ArticleNarrationRequest
    {
        [JsonPropertyName("createdYear")] public int CreatedYear { get; set; }
        [JsonPropertyName("titleShrinked")] public string TitleShrinked { get; set; }
        [JsonPropertyName("s3NarrationDir")] public string S3NarrationDir { get; set; }
        [JsonPropertyName("content")] public string Content { get; set; }
    }
}