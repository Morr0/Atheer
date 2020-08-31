using AutoMapper;

namespace AtheerCore.Models
{
    // TODO deal with duplicates of this
    public class BlogPostModelDTOMappingsProfile : Profile
    {
        public BlogPostModelDTOMappingsProfile()
        {
            CreateMap<BlogPost, BlogPostReadDTO>();
        }
    }
}
