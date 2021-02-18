using System.Collections.Generic;
using Atheer.Models;

namespace Atheer.Controllers.ViewModels
{
    public class ArticleViewModel
    {
        public ArticleViewModel(Article article, IList<Tag> tags, string authorFullName)
        {
            Article = article;
            Tags = tags;
            AuthorFullName = authorFullName;
        }

        public Article Article { get; set; }
        public IList<Tag> Tags { get; set; }
        public string AuthorFullName { get; set; }
    }
}