using System;

namespace Atheer.Controllers.Article.Models
{
    public class StrippedArticleViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool ForceFullyUnlisted { get; set; }
    }
}