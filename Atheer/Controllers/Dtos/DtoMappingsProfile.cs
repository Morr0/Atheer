using Atheer.Models;
using AutoMapper;

namespace Atheer.Controllers.Dtos
{
    public class DtoMappingsProfile : Profile
    {
        public DtoMappingsProfile()
        {
            CreateMap<BlogPostEditDto, BlogPost>();
        }
    }
}