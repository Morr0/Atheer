using Atheer.Controllers.Articles.Models;
using Atheer.Models;

namespace Atheer.ViewComponents
{
    public class ArticleEditLinkModel
    {
        public ArticleEditLinkModel(Article article, string currentUserId)
        {
            CreatedYear = article.CreatedYear;
            TitleShrinked = article.TitleShrinked;
            CurrentUserId = currentUserId;
        }

        public ArticleEditLinkModel(StrippedArticleViewModel article, string currentUserId)
        {
            CreatedYear = article.CreatedYear;
            TitleShrinked = article.TitleShrinked;
            CurrentUserId = currentUserId;
        }
        
        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }

        public string CurrentUserId { get; set; }
        
    }
}