using AtheerCore.Models;
using System.Threading.Tasks;

namespace AtheerBlogWriterBackend.Services
{
    public interface IBlogEditorService
    {
        Task<BlogPost> AddPost(BlogPostWriteDTO writeDTO);
        Task UpdateExistingPost(BlogPostUpdateDTO updateDTO);
        Task<bool> DeleteExistingPost(int year, string title);
    }
}
