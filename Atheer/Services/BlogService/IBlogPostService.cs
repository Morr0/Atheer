using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
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
        Task AddPost(BlogPostEditViewModel post, string userId);
        Task Update(BlogPostEditViewModel post);

        Task<bool> AuthorizedFor(BlogPostPrimaryKey key, string userId);
    }
}
