using System.Collections.Generic;
using AtheerBackend.Models;

namespace AtheerBackend.Services.BlogService
{
    // A wrapper on what will the service return
    // This is a cleaner choice to tuples
    public class BlogRepositoryBlogResponse
    {
        public BlogRepositoryBlogResponse(int size)
        {
            Posts = new List<BlogPost>(size);
        }

        public List<BlogPost> Posts { get; set; }

        public PostsPaginationPrimaryKey PaginationHeader { get; set; }
    }
}
