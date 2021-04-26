using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Article.Queries
{
    public class ArticlesQuery
    {
        [Range(1, int.MaxValue)]
        public int Year { get; set; }

        [MinLength(1)]
        public string Tag { get; set; }
        
        public bool Oldest { get; set; }
    }
}