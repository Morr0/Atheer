using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;

namespace Atheer.Services.ArticlesService
{
    public interface IArticleService
    {
        Task<ArticleResponse> Get(int amount, string userId = null);

        Task<ArticleResponse> GetByYear(int year, int amount, string userId = null);

        Task<ArticleViewModel> GetSpecific(ArticlePrimaryKey primaryKey);
        
        Task Like(ArticlePrimaryKey primaryKey);
        Task Share(ArticlePrimaryKey primaryKey);

        Task Delete(ArticlePrimaryKey key);
        Task Add(ArticleEditViewModel articleEditViewModel, string userId);
        Task Update(ArticleEditViewModel article);

        Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId);
    }
}
