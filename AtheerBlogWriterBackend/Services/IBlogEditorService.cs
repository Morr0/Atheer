using AtheerCore.Models;
using System.Threading.Tasks;

namespace AtheerBlogWriterBackend.Services
{
    public interface IBlogEditorService
    {
        Task<BlogPost> AddPost(BlogPostWriteDTO writeDTO);
    }
}
