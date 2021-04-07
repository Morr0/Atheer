using System.Collections.Generic;

namespace Atheer.Models
{
    public class ArticleSeries
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Finished { get; set; }
        public string DateCreated { get; set; }
        public string AuthorId { get; set; }

        public IList<Article> Articles { get; set; }
        public int ArticlesCount { get; set; }
    }
}