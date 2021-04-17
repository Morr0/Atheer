using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Article.Requests
{
    public class CompletedArticleNarrationRequest
    {
        [Required] public int CreatedYear { get; set; }
        [Required] public string TitleShrinked { get; set; }
        [Required] public string S3BucketKey { get; set; }
    }
}