using System.ComponentModel.DataAnnotations;

namespace Atheer.Services.ArticlesService
{
    public class ArticlePrimaryKey
    {
        public ArticlePrimaryKey() {}

        public ArticlePrimaryKey(string id)
        {
            Id = id;
        }

        public ArticlePrimaryKey(int createdYear, string titleShrinked)
        {
            CreatedYear = createdYear;
            TitleShrinked = titleShrinked;
        }

        public string Id
        {
            get
            {
                return string.IsNullOrEmpty(id)
                    ? $"{CreatedYear.ToString()}-{TitleShrinked}"
                    : id;
            }
            set
            {
                id = value;
            }
        }

        private string id = null;

        [Required] public int CreatedYear { get; set; }
        [Required] public string TitleShrinked { get; set; }

        public override string ToString() => $"{CreatedYear.ToString()}-{TitleShrinked}";
    }
}