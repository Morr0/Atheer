using AtheerBackend.Controllers.Headers;
using AtheerCore.Models;
using System.Threading.Tasks;

namespace AtheerBackend.Services
{
    public interface IBlogRepository
    {
        Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null);

        Task<BlogPost> Get(int year, string title);
    }
}
