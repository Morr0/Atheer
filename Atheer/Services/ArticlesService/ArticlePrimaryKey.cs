using System.ComponentModel.DataAnnotations;

namespace Atheer.Services.ArticlesService
{
    public class ArticlePrimaryKey
    {
        public ArticlePrimaryKey() {}

        public ArticlePrimaryKey(int createdYear, string titleShrinked)
        {
            CreatedYear = createdYear;
            TitleShrinked = titleShrinked;
        }

        public string Id { get; set; }
        
        [Required] public int CreatedYear { get; set; }
        [Required] public string TitleShrinked { get; set; }

        public override string ToString() => $"{CreatedYear.ToString()}-{TitleShrinked}";
    }
}