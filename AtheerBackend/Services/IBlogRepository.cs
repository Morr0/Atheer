using AtheerBackend.Controllers.Headers;
using AtheerCore.Models;
using System.Threading.Tasks;

namespace AtheerBackend.Services
{
    public interface IBlogRepository
    {
        Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null);

        Task<BlogRepositoryBlogResponse> GetByYear(int year, int amount, 
            PostsPaginationPrimaryKey paginationHeader = null);

        Task<BlogPost> Get(int year, string title);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="titleShrinked"></param>
        /// <returns>Null -> could not like for whatever reason</returns>
        Task<BlogPost> Like(int year, string titleShrinked);

        Task<BlogPost> Share(int year, string titleShrinked);
    }
}
