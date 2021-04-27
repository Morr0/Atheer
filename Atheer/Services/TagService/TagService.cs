using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Repositories;
using Atheer.Repositories.Junctions;
using Atheer.Services.ArticlesService;
using Microsoft.EntityFrameworkCore;

namespace Atheer.Services.TagService
{
    public class TagService : ITagService
    {
        private readonly TagFactory _tagFactory;
        private readonly Data _context;

        public TagService(TagFactory tagFactory, Data context)
        {
            _tagFactory = tagFactory;
            _context = context;
        }
        
        public async Task AddOrUpdateTagsPerArticle(ArticlePrimaryKey articlePrimaryKey, IEnumerable<string> titles)
        {
            var article = await _context.Article.Where(x => x.CreatedYear == articlePrimaryKey.CreatedYear &&
                                                      x.TitleShrinked == articlePrimaryKey.TitleShrinked)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (article.Tags.Count > 0) await UpdateTags(article, titles).ConfigureAwait(false);
            else await AddTags(article, titles).ConfigureAwait(false);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task AddTags(Article article, IEnumerable<string> titles, bool setUpdated = false)
        {
            foreach (string title in titles)
            {
                var tagId = _tagFactory.GetId(title);
                var tag = await _context.Tag.Where(x => x.Id == tagId).FirstOrDefaultAsync().ConfigureAwait(false);

                if (tag is null)
                {
                    tag = _tagFactory.CreateTag(title);
                    await _context.Tag.AddAsync(tag).ConfigureAwait(false);
                }
                else if (setUpdated) _tagFactory.UpdateTag(tag);

                await _context.TagArticle.AddAsync(new TagArticle(tag, article)).ConfigureAwait(false);
            }
        }

        private Task UpdateTags(Article article, IEnumerable<string> tags)
        {
            _context.TagArticle.RemoveRange(article.Tags);

            return AddTags(article, tags, true);
        }
        
        public Task<List<BareTag>> GetTopTags(int amount, int page = 0)
        {
            var queryable = _context.PopularTag.Select(x => new BareTag
            {
                Id = x.Id,
                Title = x.Title,
                Count = x.Count
            });

            return queryable.ToListAsync();
        }

        public Task<Tag> Get(string id)
        {
            return _context.Tag.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(string id, string title)
        {
            var tag = await _context.Tag
                .FirstOrDefaultAsync(x => x.Id == id).CAF();

            tag.Title = title;
            _tagFactory.UpdateTag(tag);

            _context.Update(tag);
            await _context.SaveChangesAsync().CAF();
        }
    }
}