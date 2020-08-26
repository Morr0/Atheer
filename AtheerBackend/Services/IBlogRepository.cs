using AtheerBackend.Controllers.Headers;
using System.Threading.Tasks;

namespace AtheerBackend.Services
{
    public interface IBlogRepository
    {
        Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationHeader paginationHeader = null);
    }
}
