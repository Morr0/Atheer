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

            CreateMap<BlogPostUpdateDTO, BlogPost>()
                .ForAllMembers((opts) =>
                {
                    // Make sure to update non-null properties only
                    opts.Condition((src, dst, srcMemb) => srcMemb != null);
                });
        }
    }
}
