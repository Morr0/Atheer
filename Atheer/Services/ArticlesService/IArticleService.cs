using System.Collections.Generic;
using System.Threading.Tasks;
using Atheer.Controllers.Article.Models;
using Atheer.Controllers.Article.Requests;
using Atheer.Controllers.ArticleEdit.Models;
using Atheer.Models;

namespace Atheer.Services.ArticlesService
{
    public interface IArticleService
    {
        Task<bool> Exists(ArticlePrimaryKey key, string userId = null);
        Task<ArticleViewModel> Get(ArticlePrimaryKey primaryKey, string viewerUserId = null);
        Task<ArticlesResponse> Get(int amount, string searchQuery);
        Task<ArticlesResponse> Get(int amount, int page, int createdYear = 0, string tagId = null,
            string viewerUserId = null, string specificUserId = null, bool oldest = false);
        
        Task Like(ArticlePrimaryKey primaryKey);
        Task Share(ArticlePrimaryKey primaryKey);

        Task Delete(ArticlePrimaryKey key);
        Task<ArticlePrimaryKey> Add(ArticleEditViewModel articleEditViewModel, string userId);
        Task<ArticlePrimaryKey> Add(string userId, AddArticleRequest request);
        Task Update(ArticleEditViewModel article);
        Task Update(string userId, ArticlePrimaryKey key, UpdateArticleViewModel request);

        Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId);
        Task CompletedNarration(ArticlePrimaryKey key, string cdnUrl);
        Task<IList<ArticleSeries>> GetSeries(string userId, ArticleSeriesType articleSeriesType);
        Task AddSeries(string author, AddArticleSeries request);
        Task FinishArticleSeries(string userId, int id);
    }
}
