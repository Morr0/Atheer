using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Repositories;
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
        
        public async Task AddOrUpdateTagsPerArticle(ArticlePrimaryKey articlePrimaryKey, IEnumerable<string> tags)
        {
            var article = await _context.Article.Where(x => x.CreatedYear == articlePrimaryKey.CreatedYear &&
                                                      x.TitleShrinked == articlePrimaryKey.TitleShrinked)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}