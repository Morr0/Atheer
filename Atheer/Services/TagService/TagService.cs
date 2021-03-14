using System.Collections.Generic;
using System.Threading.Tasks;
using Atheer.Repositories;
using Atheer.Services.ArticlesService;

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
            throw new System.NotImplementedException();
        }
    }
}