using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Atheer.Controllers.Article.Models;
using Atheer.Controllers.Article.Requests;
using Atheer.Controllers.ArticleEdit.Models;
using Atheer.Controllers.Articles.Models;
using Atheer.Exceptions;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Repositories.Junctions;
using Atheer.Services.ArticlesService.Exceptions;
using Atheer.Services.ArticlesService.Models;
using Atheer.Services.FileService;
using Atheer.Services.QueueService;
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
            var articles = await _context.Article.AsNoTracking().Where(x => x.SearchVector.Matches(searchQuery))
                .OrderByDescending(x => x.CreationDate)
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
                ? queryable.Where(x => x.Unlisted == false && x.Draft == false && x.Scheduled == false)
                // Registered user viewing all articles
                : queryable.Where(x =>
                    (x.AuthorId == viewerUserId) ||
                    (x.AuthorId != viewerUserId && x.Unlisted == false && x.Draft == false && x.Scheduled == false));

            // Viewing specific user's articles
            if (!string.IsNullOrEmpty(specificUserId))
            {
                queryable = queryable.Where(x => x.AuthorId == specificUserId);
            }

            if (createdYear != 0) queryable = queryable.Where(x => x.CreatedYear == createdYear);

            int skip = amount * page;

            // Order by ASC or DESC depending on the user
            queryable = (oldest
                    ? queryable.OrderBy(x => x.CreationDate)
                    : queryable.OrderByDescending(x => x.CreationDate))
                .Skip(skip)
                .Take(amount);

            var list = await queryable.ToStrippedArticles()
                .ToListAsync().ConfigureAwait(false);

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
                OldestArticles = oldest
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

        private static async Task<bool> HasAnyMoreArticles(int amount, List<StrippedArticleViewModel> list, IQueryable<Article> queryable, int skip)
        {
            return list.Count >= amount && await queryable.Skip(skip).AnyAsync().ConfigureAwait(false);
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
                .Include(x => x.Series)
                .FirstOrDefaultAsync(x => x.CreatedYear == key.CreatedYear && x.TitleShrinked == key.TitleShrinked)
                .ConfigureAwait(false);
            
            if (article is null) return null;
            if (article.Draft && article.AuthorId != viewerUserId) return null;

            if (article.Scheduled && article.AuthorId != viewerUserId) return null;

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
                    .OrderBy(x => x.CreationDate)
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

        public async Task<ArticlePrimaryKey> Add(ArticleEditViewModel articleEditViewModel, string userId)
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

            return new ArticlePrimaryKey(article.CreatedYear, article.TitleShrinked);
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
                throw new FailedOperationException();
            }

            return key;
        }

        private async Task CreateTagArticles(Article article, IList<Tag> tags)
        {
            foreach (var tag in tags)
            {
                await _context.TagArticle.AddAsync(new TagArticle(tag, article)).ConfigureAwait(false);
            }
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
            string contentChecksumPreUpdate = ChecksumAlgorithm.ComputeMD5Checksum(article.Content);

            _mapper.Map(articleEditViewModel, article);
            _articleFactory.SetUpdated(article, articleEditViewModel.Unschedule);
            
            await EnsureRequestOfNarrationIfNarratable(article, contentChecksumPreUpdate).ConfigureAwait(false);

            await using var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false);

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

        public async Task AddSeries(string authorId, AddArticleSeries request)
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
    }
}