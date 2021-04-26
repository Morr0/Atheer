using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Article.Queries
{
    public class JsonFeedPageQuery
    {
        [Range(0, int.MaxValue)] public int Page { get; set; }
    }
}