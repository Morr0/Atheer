using System;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
using Atheer.Repositories.Blog;
using AutoMapper;

namespace Atheer.Services.BlogService
{
    public class ArticleService : IArticleService
    {
        private readonly ArticleRepository _repository;
        private readonly IMapper _mapper;
        private readonly ArticleFactory _factory;

        public ArticleService(ArticleRepository repository, IMapper mapper, ArticleFactory factory)
        {
            _repository = repository;
            _mapper = mapper;
            _factory = factory;
        }

        public async Task<ArticleResponse> Get(int amount, ArticlePaginationPrimaryKey paginationHeader = null,
            string userId = null)
        {
            var response = await _repository.GetMany(amount, paginationHeader, false).ConfigureAwait(false);
            response.Articles = response.Articles.Where(article => article.HasAccessTo(userId)).ToList();
            return response;
        }

        public async Task<ArticleResponse> GetByYear(int year, int amount, 
            ArticlePaginationPrimaryKey paginationHeader = null, string userId = null)
        {
            var response = await _repository.GetMany(year, amount, 
                paginationHeader, false).ConfigureAwait(false);
            response.Articles = response.Articles.Where(article => article.HasAccessTo(userId)).ToList();
            return response;
        }

        public Task<Article> GetSpecific(ArticlePrimaryKey primaryKey)
        {
            return _repository.Get(primaryKey);
        }

        public Task<Article> Like(ArticlePrimaryKey primaryKey)
        {
            return UpdateRecord(primaryKey, UpdateArticleOperation.UpdateLikes);
        }

        public Task<Article> Share(ArticlePrimaryKey primaryKey)
        {
            return UpdateRecord(primaryKey, UpdateArticleOperation.UpdateShares);
        }

        private Task<Article> UpdateRecord(ArticlePrimaryKey primaryKey, UpdateArticleOperation operation)
        {
            string propertyToIncrement =
                operation == UpdateArticleOperation.UpdateLikes ? nameof(Article.Likes) : nameof(Article.Shares);
            string conditionProperty =
                operation == UpdateArticleOperation.UpdateLikes ? nameof(Article.Likeable) : nameof(Article.Shareable);

            return _repository.IncrementSpecificPropertyIf(primaryKey, propertyToIncrement, conditionProperty);
        }

        public Task Delete(ArticlePrimaryKey key)
        {
            return _repository.Delete(key);
        }

        public Task<Article> Update(ArticlePrimaryKey key, Article newArticle)
        {
            return _repository.Update(key, newArticle);
        }

        public async Task Add(ArticleEditViewModel articleEditViewModel, string userId)
        { 
            var article = _factory.Create(ref articleEditViewModel, userId);
            string titleShrinked = article.TitleShrinked;
            
            // Check that no other article has same titleShrinked, else generate a new titleShrinked
            var key = new ArticlePrimaryKey(article.CreatedYear, titleShrinked);
            while ((await GetSpecific(key).ConfigureAwait(false)) is not null)
            {
                titleShrinked = RandomiseExistingShrinkedTitle(ref titleShrinked);
                key.TitleShrinked = titleShrinked;
            }

            articleEditViewModel.CreatedYear = article.CreatedYear;
            articleEditViewModel.TitleShrinked = article.TitleShrinked = titleShrinked;

            await _repository.Add(article).ConfigureAwait(false);
        }

        private string RandomiseExistingShrinkedTitle(ref string existingTitleShrinked)
        {
            return $"{existingTitleShrinked}-";
        }

        public async Task Update(ArticleEditViewModel article)
        {
            var key = new ArticlePrimaryKey(article.CreatedYear, article.TitleShrinked);
            var oldArticle = await GetSpecific(key).ConfigureAwait(false);
            var newArticle = _mapper.Map(article, oldArticle);
            
            newArticle.LastUpdatedDate = DateTime.UtcNow.ToString();

            await _repository.Update(newArticle).ConfigureAwait(false);
        }

        public async Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId)
        {
            var article = await GetSpecific(key).ConfigureAwait(false);
            if (article is null) return false;

            return article.AuthorId == userId;
        }
    }
}