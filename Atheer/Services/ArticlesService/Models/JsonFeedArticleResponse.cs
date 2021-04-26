using System.Collections.Generic;
using Atheer.Models;

namespace Atheer.Services.ArticlesService.Models
{
    public class JsonFeedArticleResponse
    {
        public bool AnyNext { get; set; }
        public List<Article> Articles { get; set; }
    }
}