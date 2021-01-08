using Atheer.Controllers.ViewModels;
using Atheer.Models;
using AutoMapper;

namespace Atheer.Utilities
{
    public class ModelMappingsProfile : Profile
    {
        public ModelMappingsProfile()
        {
            TakeCareOfViewModels();
        }
        
        void TakeCareOfViewModels()
        {
            // Post
            CreateMap<BlogPostEditViewModel, BlogPost>().ReverseMap();
            
            // User
            CreateMap<RegisterViewModel, User>();
        }
    }
}