using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Exceptions;
using Atheer.Models;
using Atheer.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Services.ArticlesService
{
    public class ArticleService : IArticleService
    {
        // private readonly ArticleRepository _repository;
        private readonly IMapper _mapper;
        private readonly ArticleFactory _articleFactory;
        private readonly TagFactory _tagFactory;
        private readonly Data _context;

        public ArticleService(IMapper mapper, ArticleFactory articleFactory, TagFactory tagFactory,Data data)
        {
            _mapper = mapper;
            _articleFactory = articleFactory;
            _tagFactory = tagFactory;
            _context = data;
        }

        public async Task<ArticleResponse> Get(int amount, string userId = null)
        {
            // TODO implement user.HasAccess
            var list = await _context.Article.AsNoTracking()
                .Where(x => x.Unlisted == false && x.Draft == false)
                .Take(amount)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync().ConfigureAwait(false);
            return new ArticleResponse(amount)
            {
                Articles = list
            };
        }

        public async Task<ArticleResponse> GetByYear(int year, int amount, string userId = null)
        {
            // TODO implement user.HasAccess
            var list = await _context.Article.AsNoTracking()
                .Where(x => x.CreatedYear == year && x.Unlisted == false && x.Draft == false)
                .Take(amount)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync().ConfigureAwait(false);
            return new ArticleResponse(amount)
            {
                Articles = list
            };
        }

        public async Task<ArticleViewModel> GetSpecific(ArticlePrimaryKey key)
        {
            // TODO implement user.HasAccess
            var article = await _context.Article.AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked)
                .ConfigureAwait(false);
            if (article is null) return null;
            
            var tags = await (from ta in _context.TagArticle
                    join t in _context.Tag on ta.TagId equals t.Id
                    where 
                        ta.ArticleCreatedYear == key.CreatedYear &&
                        ta.ArticleTitleShrinked == key.TitleShrinked
                    select t
                ).ToListAsync().ConfigureAwait(false);

            return new ArticleViewModel(article, tags);
        }

        public async Task Like(ArticlePrimaryKey primaryKey)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == primaryKey.CreatedYear && x.TitleShrinked == primaryKey.TitleShrinked).ConfigureAwait(false);
            
            article.Likes++;

            _context.Entry(article).Property(x => x.Likes).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task Share(ArticlePrimaryKey primaryKey)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == primaryKey.CreatedYear && x.TitleShrinked == primaryKey.TitleShrinked).ConfigureAwait(false);
            
            article.Shares++;

            _context.Entry(article).Property(x => x.Shares).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task Delete(ArticlePrimaryKey key)
        {
            // TODO implement user.HasAccess
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                    x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked)
                .ConfigureAwait(false);
            
            var associatedTagArticles = await _context.TagArticle.AsNoTracking().Where(x =>
                    x.ArticleCreatedYear == article.CreatedYear && x.ArticleTitleShrinked == article.TitleShrinked)
                .ToListAsync().ConfigureAwait(false);

            await using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                _context.TagArticle.RemoveRange(associatedTagArticles);
                _context.Article.Remove(article);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // TODO log failed transaction
                throw new FailedOperationException();
            }
        }

        public async Task Add(ArticleEditViewModel articleEditViewModel, string userId)
        { 
            var article = _articleFactory.Create(ref articleEditViewModel, userId);
            string titleShrinked = article.TitleShrinked;
            
            // Check that no other article has same titleShrinked, else generate a new titleShrinked
            var key = new ArticlePrimaryKey(article.CreatedYear, titleShrinked);
            // TODO inefficency: check for necessaries only
            while ((await GetSpecific(key).ConfigureAwait(false)) is not null)
            {
                titleShrinked = RandomiseExistingShrinkedTitle(ref titleShrinked);
                key.TitleShrinked = titleShrinked;
            }

            articleEditViewModel.CreatedYear = article.CreatedYear;
            articleEditViewModel.TitleShrinked = article.TitleShrinked = titleShrinked;

            await using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
            
            // Article
            await _context.Article.AddAsync(article).ConfigureAwait(false);

            // Tag
            var tagsTitles = articleEditViewModel.TagsAsString.Split(',');
            var tags = await AddTagsToContextIfDontExist(tagsTitles).ConfigureAwait(false);

            // TagArticle
            await CreateTagArticles(article, tags).ConfigureAwait(false);
                
            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // TODO log failed transaction
                throw new FailedOperationException();
            }
        }

        private async Task CreateTagArticles(Article article, IList<Tag> tags)
        {
            foreach (var tag in tags)
            {
                await _context.TagArticle.AddAsync(new TagArticle(tag, article)).ConfigureAwait(false);
            }
        }

        private async Task<IList<Tag>> AddTagsToContextIfDontExist(IList<string> tagsTitles)
        {
            var list = new List<Tag>(tagsTitles.Count);
            foreach (var title in tagsTitles)
            {
                string id = _tagFactory.GetId(title);
                var tag = await _context.Tag.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
                if (tag is null)
                {
                    tag = _tagFactory.CreateTag(title);
                    await _context.Tag.AddAsync(tag).ConfigureAwait(false);
                }

                list.Add(tag);
            }

            return list;
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
            _articleFactory.SetUpdated(ref article);

            await using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
            
            // Tag
            var tagsTitles = articleEditViewModel.TagsAsString.Split(',');
            var tags = await AddTagsToContextIfDontExist(tagsTitles).ConfigureAwait(false);

            // TagArticle
            await AddTagArticlesIfNotPresent(article, tags).ConfigureAwait(false);
                
            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // TODO log failed transaction
                throw new FailedOperationException();
            }
        }

        private async Task AddTagArticlesIfNotPresent(Article article, IList<Tag> tags)
        {
            // TODO take care of editing tags, removing old ones
            
            foreach (var tag in tags)
            {
                var ta = await _context.TagArticle.FirstOrDefaultAsync(
                    x => x.TagId == tag.Id &&
                    x.ArticleCreatedYear == article.CreatedYear &&
                    x.ArticleTitleShrinked == article.TitleShrinked).ConfigureAwait(false);

                if (ta is null)
                {
                    await _context.TagArticle.AddAsync(new TagArticle(tag, article)).ConfigureAwait(false);
                }
            }
        }

        public async Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId)
        {
            // TODO Rexamine what's going on here
            var article = await GetSpecific(key).ConfigureAwait(false);
            if (article is null) return false;

            return article.Article.AuthorId == userId;
        }
    }
}