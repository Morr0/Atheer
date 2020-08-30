using AtheerBlogWriterBackend.DTOs;
using AtheerCore.Models;
using System.Threading.Tasks;

namespace AtheerBlogWriterBackend.Services
{
    internal interface IBlogEditorService
    {
        Task<BlogPost> AddPost(BlogPostWriteDTO writeDTO);
    }
}
