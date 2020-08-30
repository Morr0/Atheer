using AutoMapper;

namespace AtheerCore.Models
{
    public class BlogPostModelDTOMappingsProfile : Profile
    {
        public BlogPostModelDTOMappingsProfile()
        {
            CreateMap<BlogPostWriteDTO, BlogPost>();
            CreateMap<BlogPost, BlogPostReadDTO>();
        }
    }
}
