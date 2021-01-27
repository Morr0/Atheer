using System.Collections.Generic;
using Atheer.Models;
using Atheer.Services.BlogService;

namespace Atheer.Services.BlogService
{
    // A wrapper on what will the service return
    // This is a cleaner choice to tuples
    public class ArticleResponse
    {
        public ArticleResponse(int size)
        {
            Articles = new List<Article>(size);
        }

        public List<Article> Articles { get; set; }

        public ArticlePaginationPrimaryKey PaginationHeader { get; set; }
    }
}
