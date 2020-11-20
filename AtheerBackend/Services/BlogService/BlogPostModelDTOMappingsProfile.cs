using AtheerBackend.DTOs;
using AtheerCore.Models;
using AutoMapper;

namespace AtheerBackend.Services.BlogService
{
    // TODO deal with duplicates of this
    public class BlogPostModelDTOMappingsProfile : Profile
    {
        public BlogPostModelDTOMappingsProfile()
        {
            CreateMap<BlogPost, BlogPostReadDTO>();

            CreateMap<BlogPost, BareBlogPostReadDTO>();
        }
    }
}
