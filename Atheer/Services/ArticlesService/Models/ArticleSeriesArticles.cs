using System.Collections.Generic;

namespace Atheer.Services.ArticlesService.Models
{
    public class ArticleSeriesArticles
    {
        public int? SeriesId { get; set; }
        public string SeriesTitle { get; set; }
        public ICollection<LightArticleView> Articles { get; set; }
    }
}