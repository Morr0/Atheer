using System.Linq;
using Atheer.Controllers.Article.Models;
using Atheer.Models;

namespace Atheer.Services.ArticlesService
{
    public static class ArticleServiceUtilities
    {
        internal static IQueryable<StrippedArticleViewModel> ToStrippedArticles(this IQueryable<Article> queryable)
        {
            return queryable.Select<Article, StrippedArticleViewModel>(x => new StrippedArticleViewModel
            {
                CreatedYear = x.CreatedYear,
                Description = x.Description,
                Draft = x.Draft,
                Title = x.Title,
                Unlisted = x.Unlisted,
                AuthorId = x.AuthorId,
                CreatedAt = x.CreatedAt,
                TitleShrinked = x.TitleShrinked,
                ForceFullyUnlisted = x.ForceFullyUnlisted
            });
        }
    }
}