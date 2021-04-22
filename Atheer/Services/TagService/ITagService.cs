using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atheer.Models;
using Atheer.Services.ArticlesService;

namespace Atheer.Services.TagService
{
    public interface ITagService
    {
        Task AddOrUpdateTagsPerArticle(ArticlePrimaryKey articlePrimaryKey, IEnumerable<string> titles);
        Task<List<BareTag>> GetTopTags(int amount, int page = 0);

        static string TagsToString(IEnumerable<Tag> tags)
        {
            return string.Join(", ", tags.Select(x => x.Title));
        }
    }
}