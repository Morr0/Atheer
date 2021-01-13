using Atheer.Models;

namespace Atheer.ViewComponents
{
    public class ArticleEditLinkModel
    {
        public ArticleEditLinkModel(BlogPost post, string userId)
        {
            CreatedYear = post.CreatedYear;
            TitleShrinked = post.TitleShrinked;
            UserId = userId;
        }
        
        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }

        public string UserId { get; set; }
        
    }
}