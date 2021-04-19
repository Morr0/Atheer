using System.Collections.Generic;
using Atheer.Models;
using Atheer.Services.ArticlesService.Models;

namespace Atheer.Controllers.Article.Models
{
    public class ArticleViewModel
    {
        public ArticleViewModel(Atheer.Models.Article article, IList<Tag> tags, string authorFullName, ArticleSeriesArticles seriesArticles)
        {
            Article = article;
            Tags = tags;
            AuthorFullName = authorFullName;
            SeriesArticles = seriesArticles;
        }

        public Atheer.Models.Article Article { get; }
        public IList<Tag> Tags { get; }
        public string AuthorFullName { get; }
        public ArticleSeriesArticles SeriesArticles { get; }
    }
}