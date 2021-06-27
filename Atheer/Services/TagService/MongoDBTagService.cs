using System.Collections.Generic;
using System.Threading.Tasks;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Repositories.Junctions;
using Atheer.Services.ArticlesService;
using MongoDB.Driver;
using Tag = Atheer.Models.Tag;

namespace Atheer.Services.TagService
{
    public class MongoDBTagService : ITagService
    {
        private readonly TagFactory _tagFactory;
        private readonly IMongoClient _client;

        public MongoDBTagService(TagFactory tagFactory, IMongoClient client)
        {
            _tagFactory = tagFactory;
            _client = client;
        }
        
        public async Task AddOrUpdateTagsPerArticle(ArticlePrimaryKey articlePrimaryKey, IEnumerable<string> titles)
        {
            var article = await (await _client.Article().FindAsync(x => x.Id == articlePrimaryKey.Id).CAF())
                .FirstOrDefaultAsync().CAF();

            if (article?.TagsIds?.Count > 0) await UpdateTags(article, titles).CAF();
            else await AddTags(article, titles).CAF();

            await _client.Article().FindOneAndReplaceAsync(x => x.Id == articlePrimaryKey.Id, article).CAF();
        }
        
        private async Task AddTags(Article article, IEnumerable<string> titles, bool setUpdated = false)
        {
            article.TagsIds ??= new List<string>();

            foreach (string title in titles)
            {
                var tagId = _tagFactory.GetId(title);
                var tag = await (await _client.Tag().FindAsync(x => x.Id == tagId).CAF()).FirstOrDefaultAsync().CAF();

                if (tag is null)
                {
                    tag = _tagFactory.CreateTag(title);
                    await _client.Tag().InsertOneAsync(tag).CAF();
                }
                else if (setUpdated) _tagFactory.UpdateTag(tag);

                article.TagsIds.Add(tagId);
            }
        }

        private Task UpdateTags(Article article, IEnumerable<string> tags)
        {
            article.TagsIds?.Clear();

            return AddTags(article, tags, true);
        }

        public async Task<Tag> Get(string id)
        {
            return await (await _client.Tag().FindAsync(x => x.Id == id).CAF()).FirstOrDefaultAsync().CAF();
        }

        public async Task Update(string id, string title)
        {
            var tag = await (await _client.Tag().FindAsync(x => x.Id == id).CAF()).FirstOrDefaultAsync().CAF();

            tag.Title = title;
            _tagFactory.UpdateTag(tag);

            await _client.Tag().FindOneAndReplaceAsync(x => x.Id == id, tag).CAF();
        }
    }
}