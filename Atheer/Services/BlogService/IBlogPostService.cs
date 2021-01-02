using System.Threading.Tasks;
using Atheer.Controllers.Dtos;
using Atheer.Models;

namespace Atheer.Services.BlogService
{
    public interface IBlogPostService
    {
        Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null);

        Task<BlogRepositoryBlogResponse> GetByYear(int year, int amount, 
            PostsPaginationPrimaryKey paginationHeader = null);

        Task<BlogPost> GetSpecific(BlogPostPrimaryKey primaryKey);
        
        Task<BlogPost> Like(BlogPostPrimaryKey primaryKey);
        Task<BlogPost> Share(BlogPostPrimaryKey primaryKey);

        Task Delete(BlogPostPrimaryKey key);

        Task<BlogPost> Update(BlogPostPrimaryKey key, BlogPost newPost);
        Task<BlogPost> AddPost(BlogPostEditDto post);
        Task<BlogPost> UpdatePost(BlogPostEditDto post);
    }
}
