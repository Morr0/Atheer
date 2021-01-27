using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;

namespace Atheer.Services.BlogService
{
    public interface IArticleService
    {
        Task<ArticleResponse> Get(int amount, ArticlePaginationPrimaryKey paginationHeader = null
            , string userId = null);

        Task<ArticleResponse> GetByYear(int year, int amount, 
            ArticlePaginationPrimaryKey paginationHeader = null, string userId = null);

        Task<Article> GetSpecific(ArticlePrimaryKey primaryKey);
        
        Task<Article> Like(ArticlePrimaryKey primaryKey);
        Task<Article> Share(ArticlePrimaryKey primaryKey);

        Task Delete(ArticlePrimaryKey key);

        Task<Article> Update(ArticlePrimaryKey key, Article newArticle);
        Task Add(ArticleEditViewModel articleEditViewModel, string userId);
        Task Update(ArticleEditViewModel article);

        Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId);
    }
}
