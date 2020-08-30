using AtheerCore.Models;
using AutoMapper;

namespace AtheerBlogWriterBackend.DTOs
{
    public class DTOMappingProfile : Profile
    {
        public DTOMappingProfile()
        {
            CreateMap<BlogPostWriteDTO, BlogPost>();
        }
    }
}
