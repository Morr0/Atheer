using Atheer.Controllers.ViewModels;
using Atheer.Models;

namespace Atheer.ViewComponents
{
    public class ArticleEditLinkModel
    {
        public ArticleEditLinkModel(Article article, string userId)
        {
            CreatedYear = article.CreatedYear;
            TitleShrinked = article.TitleShrinked;
            UserId = userId;
        }

        public ArticleEditLinkModel(StrippedArticleViewModel article, string userId)
        {
            CreatedYear = article.CreatedYear;
            TitleShrinked = article.TitleShrinked;
            UserId = userId;
        }
        
        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }

        public string UserId { get; set; }
        
    }
}