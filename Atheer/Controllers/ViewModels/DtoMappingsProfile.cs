using Atheer.Models;
using AutoMapper;

namespace Atheer.Controllers.ViewModels
{
    public class DtoMappingsProfile : Profile
    {
        public DtoMappingsProfile()
        {
            CreateMap<BlogPostEditViewModel, BlogPost>().ReverseMap();
        }
    }
}