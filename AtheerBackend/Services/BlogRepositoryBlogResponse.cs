using AtheerBackend.Controllers.Headers;
using AtheerCore.Models;
using System.Collections.Generic;

namespace AtheerBackend.Services
{
    // A wrapper on what will the service return
    // This is a cleaner choice to tuples
    public class BlogRepositoryBlogResponse
    {
        public BlogRepositoryBlogResponse(int size)
        {
            Posts = new List<BlogPost>(size);
            PaginationHeader = new PostsPaginationHeader();
        }

        public List<BlogPost> Posts { get; set; }

        public PostsPaginationHeader PaginationHeader { get; set; }
    }
}
