using System;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Repositories.Blog;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Services.BlogService
{
    public class ArticleService : IArticleService
    {
        // private readonly ArticleRepository _repository;
        private readonly IMapper _mapper;
        private readonly ArticleFactory _factory;
        private readonly Data _context;

        public ArticleService(ArticleRepository repository, IMapper mapper, ArticleFactory factory, Data data)
        {
            // _repository = repository;
            _mapper = mapper;
            _factory = factory;
            _context = data;
        }

        public async Task<ArticleResponse> Get(int amount, ArticlePaginationPrimaryKey paginationHeader = null,
            string userId = null)
        {
            // TODO implement user.HasAccess
            var list = await _context.Article.AsNoTracking()
                .Where(x => x.Unlisted == false && x.Draft == false)
                .Take(amount)
                .ToListAsync().ConfigureAwait(false);
            return new ArticleResponse(amount)
            {
                Articles = list
            };
            
            // var response = await _repository.GetMany(amount, paginationHeader, false).ConfigureAwait(false);
            // response.Articles = response.Articles.Where(article => article.HasAccessTo(userId)).ToList();
            // return response;
        }

        public async Task<ArticleResponse> GetByYear(int year, int amount, 
            ArticlePaginationPrimaryKey paginationHeader = null, string userId = null)
        {
            // TODO implement user.HasAccess
            var list = await _context.Article.AsNoTracking()
                .Where(x => x.CreatedYear == year && x.Unlisted == false && x.Draft == false)
                .Take(amount)
                .ToListAsync().ConfigureAwait(false);
            return new ArticleResponse(amount)
            {
                Articles = list
            };
            
            // var response = await _repository.GetMany(year, amount, 
            //     paginationHeader, false).ConfigureAwait(false);
            // response.Articles = response.Articles.Where(article => article.HasAccessTo(userId)).ToList();
            // return response;
        }

        public async Task<Article> GetSpecific(ArticlePrimaryKey primaryKey)
        {
            // TODO implement user.HasAccess
            return await _context.Article.AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CreatedYear == primaryKey.CreatedYear && x.TitleShrinked == primaryKey.TitleShrinked)
                .ConfigureAwait(false);

            // return _repository.Get(primaryKey);
        }

        public async Task Like(ArticlePrimaryKey primaryKey)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == primaryKey.CreatedYear && x.TitleShrinked == primaryKey.TitleShrinked).ConfigureAwait(false);
            
            article.Likes++;

            _context.Entry(article).Property(x => x.Likes).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // return UpdateRecord(primaryKey, UpdateArticleOperation.UpdateLikes);
        }

        public async Task Share(ArticlePrimaryKey primaryKey)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == primaryKey.CreatedYear && x.TitleShrinked == primaryKey.TitleShrinked).ConfigureAwait(false);
            
            article.Shares++;

            _context.Entry(article).Property(x => x.Shares).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // return UpdateRecord(primaryKey, UpdateArticleOperation.UpdateShares);
        }

        // private Task<Article> UpdateRecord(ArticlePrimaryKey primaryKey, UpdateArticleOperation operation)
        // {
        //     string propertyToIncrement =
        //         operation == UpdateArticleOperation.UpdateLikes ? nameof(Article.Likes) : nameof(Article.Shares);
        //     string conditionProperty =
        //         operation == UpdateArticleOperation.UpdateLikes ? nameof(Article.Likeable) : nameof(Article.Shareable);
        //
        //     return _repository.IncrementSpecificPropertyIf(primaryKey, propertyToIncrement, conditionProperty);
        // }

        public async Task Delete(ArticlePrimaryKey key)
        {
            // TODO implement user.HasAccess
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                    x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked)
                .ConfigureAwait(false);
            _context.Article.Remove(article);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // return _repository.Delete(key);
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

            await _context.Article.AddAsync(article).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // await _repository.Add(article).ConfigureAwait(false);
        }

        private string RandomiseExistingShrinkedTitle(ref string existingTitleShrinked)
        {
            return $"{existingTitleShrinked}-";
        }

        public async Task Update(ArticleEditViewModel articleEditViewModel)
        {
            // TODO Check everything good here?
            var key = new ArticlePrimaryKey(articleEditViewModel.CreatedYear, articleEditViewModel.TitleShrinked);
            
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked).ConfigureAwait(false);
            
            _mapper.Map(articleEditViewModel, article);
            article.LastUpdatedDate = DateTime.UtcNow.ToString();

            // _context.Article.Update(article);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // await _repository.Update(newArticle).ConfigureAwait(false);
        }

        public async Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId)
        {
            // TODO Rexamine what's going on here
            var article = await GetSpecific(key).ConfigureAwait(false);
            if (article is null) return false;

            return article.AuthorId == userId;
        }
    }
}