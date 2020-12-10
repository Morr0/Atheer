using System.Collections.Generic;
using System.Threading.Tasks;
using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Models;

namespace AtheerBackend.Services.BlogService
{
    public interface IBlogPostService
    {
        Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null);
        Task<IEnumerable<BareBlogPostReadDTO>> GetBare();

        Task<BlogRepositoryBlogResponse> GetByYear(int year, int amount, 
            PostsPaginationPrimaryKey paginationHeader = null);

        Task<BlogPost> GetSpecific(BlogPostPrimaryKey primaryKey);
        
        Task<BlogPost> Like(BlogPostPrimaryKey primaryKey);
        Task<BlogPost> Share(BlogPostPrimaryKey primaryKey);

        Task Delete(BlogPostPrimaryKey key);

        Task<BlogPost> Update(BlogPostPrimaryKey key, BlogPost newPost);
    }
}
