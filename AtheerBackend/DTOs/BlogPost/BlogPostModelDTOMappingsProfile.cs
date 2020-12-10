using AutoMapper;

namespace AtheerBackend.DTOs.BlogPost
{
    public class BlogPostModelDTOMappingsProfile : Profile
    {
        public BlogPostModelDTOMappingsProfile()
        {
            CreateMap<Models.BlogPost, BlogPostReadDTO>();

            CreateMap<Models.BlogPost, BareBlogPostReadDTO>();

            CreateMap<BlogPostUpdateDTO, Models.BlogPost>();
        }
    }
}
