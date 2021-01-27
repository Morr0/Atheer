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
            // Article
            TakeCareOfArticleToFromArticleEditVm();
            
            // User
            CreateMap<RegisterViewModel, User>();
        }

        private void TakeCareOfArticleToFromArticleEditVm()
        {
            CreateMap<ArticleEditViewModel, Article>()
                .ForMember(dest => dest.Topics, opts =>
                {
                    opts.MapFrom(src 
                        => src.TopicsAsString.Split(',', StringSplitOptions.TrimEntries).ToList());
                });

            CreateMap<Article, ArticleEditViewModel>()
                .ForMember(dest => dest.TopicsAsString, opts =>
                {
                    opts.MapFrom(src => String.Join(',', src.Topics));
                });
        }
    }
}