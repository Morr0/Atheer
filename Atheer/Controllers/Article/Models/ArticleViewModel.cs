using System.Collections.Generic;
using Atheer.Models;

namespace Atheer.Controllers.Article.Models
{
    public class ArticleViewModel
    {
        public ArticleViewModel(Atheer.Models.Article article, IList<Tag> tags, string authorFullName)
        {
            Article = article;
            Tags = tags;
            AuthorFullName = authorFullName;
        }

        public Atheer.Models.Article Article { get; set; }
        public IList<Tag> Tags { get; set; }
        public string AuthorFullName { get; set; }
    }
}