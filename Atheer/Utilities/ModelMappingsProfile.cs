using System;
using System.Linq;
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
        
        private void TakeCareOfViewModels()
        {
            // Post
            TakeCareOfPostToFromPostEditVm();
            
            // User
            CreateMap<RegisterViewModel, User>();
        }

        private void TakeCareOfPostToFromPostEditVm()
        {
            CreateMap<BlogPostEditViewModel, BlogPost>()
                .ForMember(dest => dest.Topics, opts =>
                {
                    opts.MapFrom(src 
                        => src.TopicsAsString.Split(',', StringSplitOptions.None).ToList());
                });

            CreateMap<BlogPost, BlogPostEditViewModel>()
                .ForMember(dest => dest.TopicsAsString, opts =>
                {
                    opts.MapFrom(src => String.Join(',', src.Topics));
                });
        }
    }
}