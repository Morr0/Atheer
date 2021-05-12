using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Atheer.Controllers.Article.Models;
using Atheer.Controllers.Article.Requests;
using Atheer.Controllers.Series.Requests;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Services.ArticlesService.Exceptions;
using Atheer.Services.ArticlesService.Models;
using Atheer.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atheer.Services.ArticlesService
{
    public class ArticleService : IArticleService
    {
        // private readonly ArticleRepository _repository;
        private readonly IMapper _mapper;
        private readonly ArticleFactory _articleFactory;
        private readonly Data _context;
        private readonly ILogger<ArticleService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ArticleService(IMapper mapper, ArticleFactory articleFactory, Data data, ILogger<ArticleService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _mapper = mapper;
            _articleFactory = articleFactory;
            _context = data;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<ArticlesResponse> Get(int amount, string searchQuery)
        {
            var articles = await _context.Article.AsNoTracking()
                .Where(x => x.SearchVector.Matches(searchQuery) && !x.Draft && !x.Unlisted && !x.ForceFullyUnlisted)
                .OrderByDescending(x => x.CreatedAt)
                .Take(amount)
                .ToStrippedArticles()
                .ToListAsync().ConfigureAwait(false);

            return new ArticlesResponse
            {
                Articles = articles,
                Search = true,
                SearchQuery = searchQuery
            };
        }

        public async Task<ArticlesResponse> Get(int amount, int page, int createdYear = 0, string tagId = null,
            string viewerUserId = null, string specificUserId = null, bool oldest = false)
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
                tagTitle = await GetTagTitle(tagId);
                if (tagTitle is null) return null;

                GetArticlesOfSpecificTag(ref queryable, tagId);
            }

            queryable = string.IsNullOrEmpty(viewerUserId)
                // Public viewing all articles
                ? queryable.Where(x => x.EverPublished && !x.Unlisted && !x.Draft && !x.ForceFullyUnlisted)
                // Registered user viewing all articles
                : queryable.Where(x =>
                    (x.AuthorId == viewerUserId) ||
                    (x.AuthorId != viewerUserId && x.EverPublished && !x.Draft && !x.Unlisted && !x.ForceFullyUnlisted));

            // Viewing specific user's articles
            if (!string.IsNullOrEmpty(specificUserId))
            {
                queryable = queryable.Where(x => x.AuthorId == specificUserId);
            }

            if (createdYear != 0) queryable = queryable.Where(x => x.CreatedYear == createdYear);

            int skip = amount * page;

            // Order by ASC or DESC depending on the user
            queryable = (oldest
                    ? queryable.OrderBy(x => x.CreatedAt)
                    : queryable.OrderByDescending(x => x.CreatedAt))
                .Skip(skip)
                .Take(amount);

            var list = await queryable.ToStrippedArticles()
                .ToListAsync().ConfigureAwait(false);

            string userName = list.Count > 0
                ? await _context.User
                    .Where(x => x.Id == specificUserId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync().CAF()
                : null;

            bool hasNext = await HasAnyMoreArticles(amount, list, queryable, skip);
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
                UserId = specificUserId,
                UserName = userName,
                OldestArticles = oldest
            };
        }

        public async Task<JsonFeedArticleResponse> Get(int amount, int page)
        {
            int skip = amount * page;
            var queryable = _context.Article.AsNoTracking()
                .Where(x => x.EverPublished && !x.Unlisted && !x.Draft && !x.ForceFullyUnlisted)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(amount);
            var articles = await queryable.ToListAsync().CAF();
            bool hasNext = await HasAnyMoreArticles(amount, articles, queryable, skip);

            return new JsonFeedArticleResponse
            {
                Articles = articles,
                AnyNext = hasNext
            };
        }
        
        private async Task<string> GetTagTitle(string tagId)
        {
            return await _context.Tag.Where(x => x.Id == tagId)
                .Select(x => x.Title).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        private void GetArticlesOfSpecificTag(ref IQueryable<Article> queryable, string tagId)
        {
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

        private static async Task<bool> HasAnyMoreArticles<T>(int amount, List<T> list, IQueryable<Article> queryable, int skip)
        {
            return list.Count == amount && await queryable.AnyAsync().CAF();
        }

        private async Task<bool> Exists(ArticlePrimaryKey key)
        {
            return await _context.Article.AsNoTracking()
                .AnyAsync(x => x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked).CAF();
        }

        public async Task<ArticleViewModel> Get(ArticlePrimaryKey key, string viewerUserId = null)
        {
            var article = await _context.Article.AsNoTracking()
                .Include(x => x.Series)
                .FirstOrDefaultAsync(x => x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked)
                .CAF();
            
            if (article is null) return null;
            if ((!article.EverPublished || article.ForceFullyUnlisted || article.Draft) && article.AuthorId != viewerUserId) return null;
            // if (article.Draft && article.AuthorId != viewerUserId) return null;
            
            // Get author full name
            var author = await _context.User.AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name
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

            ArticleSeriesArticles articleSeriesArticles = new ArticleSeriesArticles();
            if (article.Series is not null)
            {
                var lightArticleViews = await _context.Article.AsNoTracking()
                    .Where(x => x.SeriesId == article.SeriesId)
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new LightArticleView
                    {
                        Title = x.Title,
                        TitleShrinked = x.TitleShrinked,
                        CreatedYear = x.CreatedYear
                    })
                    .ToListAsync().ConfigureAwait(false);

                articleSeriesArticles.SeriesId = article.Series.Id;
                articleSeriesArticles.SeriesTitle = article.Series.Title;
                articleSeriesArticles.Articles = lightArticleViews;
            }

            return new ArticleViewModel(article, tags, author.Name, articleSeriesArticles);
        }

        public async Task Like(ArticlePrimaryKey primaryKey)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == primaryKey.CreatedYear && x.TitleShrinked == primaryKey.TitleShrinked).CAF();

            if (article is null) throw new ArticleNotFoundException();
            
            article.Likes++;

            _context.Entry(article).Property(x => x.Likes).IsModified = true;
            await _context.SaveChangesAsync().CAF();
        }

        public async Task Share(ArticlePrimaryKey primaryKey)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == primaryKey.CreatedYear && x.TitleShrinked == primaryKey.TitleShrinked).CAF();
            
            if (article is null) throw new ArticleNotFoundException();
            
            article.Shares++;

            _context.Entry(article).Property(x => x.Shares).IsModified = true;
            await _context.SaveChangesAsync().CAF();
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

        public async Task<ArticlePrimaryKey> Add(string userId, AddArticleRequest request)
        {
            var article = _articleFactory.Create(request, userId);
            string titleShrinked = article.TitleShrinked;
            
            // Check that no other article has same titleShrinked, else generate a new titleShrinked
            var key = new ArticlePrimaryKey(article.CreatedYear, titleShrinked);
            while (await Exists(key).CAF())
            {
                titleShrinked = RandomiseExistingShrinkedTitle(ref titleShrinked);
                key.TitleShrinked = titleShrinked;
                article.TitleShrinked = titleShrinked;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync().CAF();
            await _context.Article.AddAsync(article).CAF();
            try
            {
                await _context.SaveChangesAsync().CAF();
                await transaction.CommitAsync().CAF();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                throw new FailedOperationException();
            }

            return key;
        }

        private string RandomiseExistingShrinkedTitle(ref string existingTitleShrinked)
        {
            return $"{existingTitleShrinked}-";
        }

        public async Task Update(string userId, ArticlePrimaryKey key, UpdateArticleViewModel request)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x =>
                x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked).CAF();
            string contentChecksumPreUpdate = ChecksumAlgorithm.ComputeMD5Checksum(article.Content);

            _mapper.Map(request, article);
            _articleFactory.SetUpdated(article);
            
            await EnsureRequestOfNarrationIfNarratable(article, contentChecksumPreUpdate).CAF();

            await using var transaction = await _context.Database.BeginTransactionAsync().CAF();

            try
            {
                await _context.SaveChangesAsync().CAF();
                await transaction.CommitAsync().CAF();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new FailedOperationException();
            }
        }

        public async Task UpdateForcefullUnlist(ArticlePrimaryKey key, bool forcefullyUnlisted)
        {
            var article = await _context.Article
                .FirstOrDefaultAsync(x => x.CreatedYear == key.CreatedYear && 
                                          x.TitleShrinked == key.TitleShrinked).CAF();
            if (article is null) return;

            article.ForceFullyUnlisted = forcefullyUnlisted;

            _context.Update(article);
            await _context.SaveChangesAsync().CAF();
        }

        private async ValueTask EnsureRequestOfNarrationIfNarratable(Article article, string contentChecksumPreUpdate)
        {
            if (!article.Narratable) return;
            
            string contentChecksumPostUpdate = ChecksumAlgorithm.ComputeMD5Checksum(article.Content);
            if (contentChecksumPreUpdate == contentChecksumPostUpdate) return;

            var narrationRequest = _articleFactory.CreateNarrationRequest(article);

            using var scope = _serviceScopeFactory.CreateScope();
            var channel = scope.ServiceProvider.GetRequiredService<Channel<ArticleNarrationRequest>>();
            await channel.Writer.WriteAsync(narrationRequest).ConfigureAwait(false);
        }

        public async Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId)
        {
            string articleAuthorId = await _context.Article.AsNoTracking()
                .Where(x => x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked)
                .Select(x => x.AuthorId)
                .FirstOrDefaultAsync().CAF();
            if (string.IsNullOrEmpty(articleAuthorId)) throw new ArticleNotFoundException(); 

            return articleAuthorId == userId;
        }

        public async Task CompletedNarration(ArticlePrimaryKey key, string cdnUrl)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x => x.CreatedYear == key.CreatedYear &&
                                                                          x.TitleShrinked == key.TitleShrinked)
                .ConfigureAwait(false);
            if (article is null || !article.Narratable) return;

            article.NarrationMp3Url = cdnUrl;

            _context.Attach(article).Property(x => x.NarrationMp3Url).IsModified = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IList<ArticleSeries>> GetSeries(string userId, ArticleSeriesType articleSeriesType)
        {
            var queryable = _context.ArticleSeries.AsNoTracking()
                .Where(x => x.AuthorId == userId);

            if (articleSeriesType == ArticleSeriesType.Finished)
            {
                queryable = queryable.Where(x => x.Finished);
            } 
            else if (articleSeriesType == ArticleSeriesType.Unfinished)
            {
                queryable = queryable.Where(x => !x.Finished);
            }

            return await queryable.ToListAsync().ConfigureAwait(false);
        }

        public async Task AddSeries(string authorId, AddSeriesRequest request)
        {
            var series = _articleFactory.CreateSeries(authorId, request.Title, request.Description);

            await _context.ArticleSeries.AddAsync(series).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task FinishArticleSeries(string userId, int id)
        {
            var series = await _context.ArticleSeries.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (series is null && series.AuthorId != userId) return;
            
            _articleFactory.FinishSeries(series);

            _context.Update(series);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<List<LightArticleSeries>> GetSeriesFor(string userId)
        {
            return _context.ArticleSeries.AsNoTracking()
                .Where(x => !x.Finished && x.AuthorId == userId)
                .Select(x => new LightArticleSeries
                {
                    Id = x.Id,
                    Title = x.Title
                })
                .ToListAsync();
        }

        public Task<ArticleSeries> GetSeries(int? id)
        {
            return _context.ArticleSeries.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}