using System.Collections.Generic;
using System.Threading.Tasks;
using Atheer.Services.ArticlesService;

namespace Atheer.Services.TagService
{
    public interface ITagService
    {
        Task AddOrUpdateTagsPerArticle(ArticlePrimaryKey articlePrimaryKey, IEnumerable<string> titles);
    }
}