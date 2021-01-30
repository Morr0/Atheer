using System.Collections.Generic;
using Atheer.Models;

namespace Atheer.Controllers.ViewModels
{
    public class ArticleViewModel
    {
        public ArticleViewModel(Article article, IList<Tag> tags)
        {
            Article = article;
            Tags = tags;
        }

        public Article Article { get; set; }
        public IList<Tag> Tags { get; set; }
    }
}