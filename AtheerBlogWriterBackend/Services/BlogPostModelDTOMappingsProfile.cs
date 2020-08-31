using AtheerCore.Models;
using AutoMapper;

namespace AtheerBlogWriteBackend.Models
{
    public class BlogPostModelDTOMappingsProfile : Profile
    {
        public BlogPostModelDTOMappingsProfile()
        {
            CreateMap<BlogPostWriteDTO, BlogPost>();
            CreateMap<BlogPost, BlogPostReadDTO>();

            CreateMap<BlogPostUpdateDTO, BlogPost>();
        }
    }
}
