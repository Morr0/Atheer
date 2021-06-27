using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Atheer.Controllers.Article.Models;
using Atheer.Controllers.Article.Requests;
using Atheer.Controllers.Series.Requests;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services.ArticlesService.Models;
using Atheer.Utilities;
using AutoMapper;
using MongoDB.Driver;
using Tag = Atheer.Models.Tag;

namespace Atheer.Services.ArticlesService
{
    public class MongoDBArticleService : IArticleService
    {
        private readonly IMongoClient _client;
        private readonly ArticleFactory _articleFactory;
        private readonly IMapper _mapper;

        public MongoDBArticleService(IMongoClient client, ArticleFactory articleFactory, IMapper mapper)
        {
            _client = client;
            _articleFactory = articleFactory;
            _mapper = mapper;
        }
        
        public async Task<ArticleViewModel> Get(ArticlePrimaryKey primaryKey, string viewerUserId = null)
        {
            var article = await (await _client.Article().FindAsync(x => x.Id == primaryKey.Id).CAF())
                .FirstOrDefaultAsync().CAF();
            
            if (article is null) return null;
            if ((!article.EverPublished || article.ForceFullyUnlisted || article.Draft) && article.AuthorId != viewerUserId) return null;

            var author = await (await _client.User().FindAsync(x => x.Id == article.AuthorId).CAF())
                .FirstOrDefaultAsync().CAF();
            var tags = (await (await _client.Tag().FindAsync(x => article.TagsIds.Contains(x.Id)).CAF()).ToListAsync()
                .CAF());
            // TODO enable article series to bring articles in same series

            return new ArticleViewModel(article, tags, author.Name, new ArticleSeriesArticles());
        }

        public async Task<ArticlesResponse> Get(int amount, int page, int createdYear = 0, string tagId = null, string viewerUserId = null,
            string specificUserId = null, bool oldest = false)
        {
            var articles = _client.Article();
            
            string tagTitle = string.IsNullOrEmpty(tagId) ? null : await GetTagTitle(tagId).CAF();
            // IAsyncCursor<Article> queryable = null;
            
            // queryable = string.IsNullOrEmpty(viewerUserId)
            //     // Public viewing all articles
            //     ? await articles.FindAsync(x => x.EverPublished && !x.Unlisted && !x.Draft && !x.ForceFullyUnlisted).CAF()
            //     // Registered user viewing all articles
            //     : await articles.FindAsync().CAF()

            int skip = amount * page;
            List<StrippedArticleViewModel> articleViewModels = null;
            
            // Public viewing all articles
            if (string.IsNullOrEmpty(viewerUserId))
            {
                var list = await (await articles
                    .FindAsync(x => x.EverPublished && !x.Unlisted && !x.Draft && !x.ForceFullyUnlisted, new FindOptions<Article>()
                    {
                        Limit = amount,
                        Skip = skip,
                        Sort = oldest 
                            ? Builders<Article>.Sort.Ascending(x => x.CreatedAt)
                            : Builders<Article>.Sort.Descending(x => x.CreatedAt),
                        Projection = Builders<Article>.Projection.Combine(
                            Builders<Article>.Projection.Include(x => x.Id),
                            Builders<Article>.Projection.Include(x => x.Description),
                            Builders<Article>.Projection.Include(x => x.Draft),
                            Builders<Article>.Projection.Include(x => x.Title),
                            Builders<Article>.Projection.Include(x => x.Unlisted),
                            Builders<Article>.Projection.Include(x => x.AuthorId),
                            Builders<Article>.Projection.Include(x => x.CreatedAt),
                            Builders<Article>.Projection.Include(x => x.ForceFullyUnlisted)
                            ),
                    }).CAF()).ToListAsync().CAF();
                articleViewModels = list.ToStrippedArticles().ToList();
            }
            // Registered user viewing all articles
            else
            {
                var list = await (await articles.FindAsync(x =>
                        (x.AuthorId == viewerUserId) ||
                        (x.AuthorId != viewerUserId && x.EverPublished && !x.Draft && !x.Unlisted &&
                         !x.ForceFullyUnlisted),
                    new FindOptions<Article>()
                    {
                        Limit = amount,
                        Skip = skip,
                        Sort = oldest 
                            ? Builders<Article>.Sort.Ascending(x => x.CreatedAt)
                            : Builders<Article>.Sort.Descending(x => x.CreatedAt),
                        Projection = Builders<Article>.Projection.Combine(
                            Builders<Article>.Projection.Include(x => x.Id),
                            Builders<Article>.Projection.Include(x => x.Description),
                            Builders<Article>.Projection.Include(x => x.Draft),
                            Builders<Article>.Projection.Include(x => x.Title),
                            Builders<Article>.Projection.Include(x => x.Unlisted),
                            Builders<Article>.Projection.Include(x => x.AuthorId),
                            Builders<Article>.Projection.Include(x => x.CreatedAt),
                            Builders<Article>.Projection.Include(x => x.ForceFullyUnlisted)
                        ),
                    }).CAF()).ToListAsync().CAF();
                articleViewModels = list.ToStrippedArticles().ToList();
            }
            
            // TODO add pagination
            // TODO add user name
            string userName = null;

            return new ArticlesResponse
            {
                Articles = articleViewModels,
                AnyNext = false,
                AnyPrevious = false,
                Year = createdYear,
                CurrentPage = page,
                TagId = tagId,
                TagTitle = tagTitle,
                UserId = specificUserId,
                UserName = userName,
                OldestArticles = oldest
            };
        }
        
        private async Task<string> GetTagTitle(string tagId)
        {
            var tags = _client.Tag();
            return (await (await tags.FindAsync(x => x.Id == tagId).CAF())
                .FirstOrDefaultAsync().CAF()).Title;
        }

        public async Task Like(ArticlePrimaryKey primaryKey)
        {
            await _client.Article().FindOneAndUpdateAsync(x => x.Id == primaryKey.Id,
                Builders<Article>.Update.Inc(x => x.Likes, 1)).CAF();
        }

        public async Task Share(ArticlePrimaryKey primaryKey)
        {
            await _client.Article().FindOneAndUpdateAsync(x => x.Id == primaryKey.Id,
                Builders<Article>.Update.Inc(x => x.Shares, 1)).CAF();
        }

        public async Task Delete(ArticlePrimaryKey key)
        {
            await _client.Article().DeleteOneAsync(x => x.Id == key.Id).CAF();
        }

        private async Task<bool> Exists(string articleId)
        {
           return await (await _client.Article().FindAsync(x => x.Id == articleId).CAF()).AnyAsync().CAF();
        }

        public async Task<ArticlePrimaryKey> Add(string userId, AddArticleRequest request)
        {
            var article = _articleFactory.Create(request, userId);
            // Check that no other article has same id, else will generate another
            while (await Exists(article.Id).CAF())
            {
                article.Id = $"{article.Id}-";
            }

            await _client.Article().InsertOneAsync(article).CAF();
            return new ArticlePrimaryKey
            {
                Id = article.Id,
            };
        }

        public async Task Update(string userId, ArticlePrimaryKey key, UpdateArticleViewModel request)
        {
            var article = await (await _client.Article().FindAsync(x => x.Id == key.Id).CAF())
                .FirstOrDefaultAsync().CAF();
            string contentChecksumPreUpdate = ChecksumAlgorithm.ComputeMD5Checksum(article.Content);

            _mapper.Map(request, article);
            _articleFactory.SetUpdated(article);
            
            // TODO enable narration
            await _client.Article().FindOneAndReplaceAsync(x => x.Id == key.Id, article).CAF();
        }

        public async Task UpdateForcefullUnlist(ArticlePrimaryKey key, bool forcefullyUnlisted)
        {
            await _client.Article().UpdateOneAsync(x => x.Id == key.Id,
                Builders<Article>.Update.Set(x => x.ForceFullyUnlisted, forcefullyUnlisted)).CAF();
        }

        public async Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId)
        {
            var article = await (await _client.Article().FindAsync(x => x.Id == key.Id, new FindOptions<Article>()
            {
                Projection = Builders<Article>.Projection.Include(x => x.AuthorId)
            }).CAF()).FirstOrDefaultAsync().CAF();

            return article.AuthorId == userId;
        }

        public Task CompletedNarration(ArticlePrimaryKey key, string cdnUrl)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IList<ArticleSeries>> GetSeries(string userId, ArticleSeriesType articleSeriesType)
        {
            Expression<Func<ArticleSeries, bool>> filter = (x) => articleSeriesType == ArticleSeriesType.Finished 
                ? x.Finished
                : articleSeriesType == ArticleSeriesType.Unfinished 
                    ? !x.Finished
                    : x.Finished || !x.Finished;
            return await (await _client.ArticleSeries().FindAsync(filter).CAF()).ToListAsync();
        }

        public async Task AddSeries(string author, AddSeriesRequest request)
        {
            var series = _articleFactory.CreateSeries(author, request.Title, request.Description);
            await _client.ArticleSeries().InsertOneAsync(series).CAF();
        }

        public async Task FinishArticleSeries(string userId, int id)
        {
            await _client.ArticleSeries().FindOneAndUpdateAsync(x => x.SeriesId == id.ToString(),
                Builders<ArticleSeries>.Update.Set(x => x.Finished, true)).CAF();
        }

        public async Task<List<LightArticleSeries>> GetSeriesFor(string userId)
        {
            var series = await (await _client.ArticleSeries().FindAsync(x => x.AuthorId == userId).CAF())
                .ToListAsync().CAF();

            return series.Select(x => new LightArticleSeries
            {
                Title = x.Title,
                Id = x.Id
            }).ToList();
        }

        public async Task<ArticleSeries> GetSeries(int? id)
        {
            return await (await _client.ArticleSeries().FindAsync(x => x.SeriesId == id.ToString()).CAF())
                .FirstOrDefaultAsync().CAF();
        }
    }
}