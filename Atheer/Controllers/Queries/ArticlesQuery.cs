using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Queries
{
    public class ArticlesQuery
    {
        [Range(1, int.MaxValue)]
        public int Year { get; set; }

        [MinLength(1)]
        public string Tag { get; set; }

        [Range(0, uint.MaxValue)]
        public int Page { get; set; }

        public bool Empty()
        {
            return Year == 0 && string.IsNullOrEmpty(Tag) && Page == 0;
        }
    }
}