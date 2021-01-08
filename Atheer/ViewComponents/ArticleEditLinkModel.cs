using Atheer.Models;

namespace Atheer.ViewComponents
{
    public class ArticleEditLinkModel
    {
        public ArticleEditLinkModel(BlogPost post)
        {
            CreatedYear = post.CreatedYear;
            TitleShrinked = post.TitleShrinked;
        }
        
        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }
        
    }
}