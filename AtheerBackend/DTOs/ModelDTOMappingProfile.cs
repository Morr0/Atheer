using AtheerCore.Models;
using AutoMapper;

namespace AtheerBackend.DTO
{
    public class ModelDTOMappingProfile : Profile
    {
        public ModelDTOMappingProfile()
        {
            CreateMap<BlogPost, BlogPostReadDTO>();
        }
    }
}
