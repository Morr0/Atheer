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
using Microsoft.Extensions.Logging;

namespace Atheer.Services.ArticlesService
{
    public class ArticleService : IArticleService
    {
        // private readonly ArticleRepository _repository;
        private readonly IMapper _mapper;
        private readonly ArticleFactory _articleFactory;
        private readonly TagFactory _tagFactory;
        private readonly Data _context;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(IMapper mapper, ArticleFactory articleFactory, TagFactory tagFactory, Data data, ILogger<ArticleService> logger)
        {
            _mapper = mapper;
            _articleFactory = articleFactory;
            _tagFactory = tagFactory;
            _context = data;
            _logger = logger;
        }

        public async Task<ArticlesResponse> Get(int amount, int page, int createdYear = 0, string tagId = null,
            string viewerUserId = null, string specificUserId = null)
        {
            string tagTitle = null;

            IQueryable<Article> queryable = null;
            if (string.IsNullOrEmpty(tagId))
            {
                queryable = _context.Article.AsNoTracking();
            }
            // Specific tag
            else
            {
                tagTitle = await _context.Tag.Where(x => x.Id == tagId)
                    .Select(x => x.Title).FirstOrDefaultAsync().ConfigureAwait(false);
                if (tagTitle is null) return null;
                
                queryable = from ta in _context.TagArticle.AsNoTracking()
                    join t in _context.Tag.AsNoTracking() on ta.TagId equals t.Id
                    join a in _context.Article.AsNoTracking() on
                        new
                        {
                            ta.ArticleCreatedYear,
                            ta.ArticleTitleShrinked
                        } equals 
                        new
                        {
                            ArticleCreatedYear = a.CreatedYear,
                            ArticleTitleShrinked = a.TitleShrinked
                        }
                    where t.Id == tagId
                    select a;
            }
            
            queryable = string.IsNullOrEmpty(viewerUserId)
                // Public viewing all articles
                ? queryable.Where(x => x.Unlisted == false && x.Draft == false)
                // Registered user viewing all articles
                : queryable.Where(x =>
                    (x.AuthorId == viewerUserId) ||
                    (x.AuthorId != viewerUserId && x.Unlisted == false && x.Draft == false));

            // Viewing specific user's articles
            if (!string.IsNullOrEmpty(specificUserId))
            {
                queryable = queryable.Where(x => x.AuthorId == specificUserId);
            }

            if (createdYear != 0) queryable = queryable.Where(x => x.CreatedYear == createdYear);
            
            int skip = amount * page;
            queryable = queryable
                .OrderByDescending(x => x.CreationDate)
                .Skip(skip)
                .Take(amount);

            var list = await queryable.Select<Article, StrippedArticleViewModel>(x => new StrippedArticleViewModel
            {
                CreatedYear = x.CreatedYear,
                Description = x.Description,
                Draft = x.Draft,
                Title = x.Title,
                Unlisted = x.Unlisted,
                AuthorId = x.AuthorId,
                CreationDate = x.CreationDate,
                TitleShrinked = x.TitleShrinked
            }).ToListAsync().ConfigureAwait(false);
            
            // Checks whether any next first by seeing if the returned list is less than the amount of page then surely
            // no more exists otherwise will seek and see
            bool hasNext = list.Count >= amount && await queryable.Skip(skip).AnyAsync().ConfigureAwait(false);
            bool hasPrevious = skip > 0;

            return new ArticlesResponse
            {
                Articles = list,
                AnyNext = hasNext,
                AnyPrevious = hasPrevious,
                Year = createdYear,
                CurrentPage = page,
                TagId = tagId,
                TagTitle = tagTitle,
                UserId = specificUserId
            };
        }

        public async Task<bool> Exists(ArticlePrimaryKey key, string userId = null)
        {
            var article = await _context.Article.AsNoTracking()
                .Where(x => x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked)
                .Select(x => new
                {
                    x.Draft,
                    x.AuthorId
                })
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (article is null) return false;
            return !article.Draft || article.AuthorId == userId;
        }

        public async Task<ArticleViewModel> Get(ArticlePrimaryKey key, string viewerUserId = null)
        {
            var article = await _context.Article.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked)
                .ConfigureAwait(false);
            
            if (article is null) return null;
            if (article.Draft && article.AuthorId != viewerUserId) return null;

            // Get author full name
            var authorFullName = await _context.User.AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName
                })
                .FirstOrDefaultAsync(x => x.Id == article.AuthorId).ConfigureAwait(false);

            // Get tags associated with article
            var tags = await (from ta in _context.TagArticle
                    join t in _context.Tag.AsNoTracking() on ta.TagId equals t.Id
                    where 
                        ta.ArticleCreatedYear == key.CreatedYear &&
                        ta.ArticleTitleShrinked == key.TitleShrinked
                    select t
                ).ToListAsync().ConfigureAwait(false);

            return new ArticleViewModel(article, tags, $"{authorFullName.FirstName} {authorFullName.LastName}");
        }

        public async Task Like(ArticlePrimaryKey primaryKey)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == primaryKey.CreatedYear && x.TitleShrinked == primaryKey.TitleShrinked).ConfigureAwait(false);

            if (article is null) throw new InvalidOperationException();
            
            article.Likes++;

            _context.Entry(article).Property(x => x.Likes).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task Share(ArticlePrimaryKey primaryKey)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == primaryKey.CreatedYear && x.TitleShrinked == primaryKey.TitleShrinked).ConfigureAwait(false);
            
            if (article is null) throw new InvalidOperationException();
            
            article.Shares++;

            _context.Entry(article).Property(x => x.Shares).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task Delete(ArticlePrimaryKey key)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                    x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked)
                .ConfigureAwait(false);

            if (article is null) return;
            
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
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new FailedOperationException();
            }
        }

        public async Task Add(ArticleEditViewModel articleEditViewModel, string userId)
        { 
            var article = _articleFactory.Create(ref articleEditViewModel, userId);
            string titleShrinked = article.TitleShrinked;
            
            // Check that no other article has same titleShrinked, else generate a new titleShrinked
            var key = new ArticlePrimaryKey(article.CreatedYear, titleShrinked);
            while (await Exists(key).ConfigureAwait(false))
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
            catch (Exception e)
            {
                _logger.LogError(e.Message);
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
            var key = new ArticlePrimaryKey(articleEditViewModel.CreatedYear, articleEditViewModel.TitleShrinked);
            
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked).ConfigureAwait(false);
            
            _mapper.Map(articleEditViewModel, article);
            _articleFactory.SetUpdated(ref article, articleEditViewModel.Unschedule);

            await using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);
            
            // Tag
            var tagsTitles = articleEditViewModel.TagsAsString.Split();
            var tags = await AddTagsToContextIfDontExist(tagsTitles).ConfigureAwait(false);

            // TagArticle
            await UpdateTagArticles(article, tags).ConfigureAwait(false);
                
            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new FailedOperationException();
            }
        }

        private async Task UpdateTagArticles(Article article, IList<Tag> tags)
        {
            // Remove existing ones
            var existingTagArticles = await _context.TagArticle
                .Where(x => x.ArticleCreatedYear == article.CreatedYear &&
                            x.ArticleTitleShrinked == article.TitleShrinked)
                .ToListAsync().ConfigureAwait(false);
            
            _context.TagArticle.RemoveRange(existingTagArticles);

            // Add new ones
            foreach (var tag in tags)
            {
                await _context.TagArticle.AddAsync(new TagArticle(tag, article)).ConfigureAwait(false);
            }
        }

        public async Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId)
        {
            var article = await Get(key).ConfigureAwait(false);
            if (article is null) return false;

            return article.Article.AuthorId == userId;
        }
    }
}