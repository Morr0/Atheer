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
            CreateMap<User, UserSettingsViewModel>();
            CreateMap<UserSettingsUpdate, User>();
        }

        private void TakeCareOfArticleToFromArticleEditVm()
        {
            CreateMap<ArticleEditViewModel, Article>();
            CreateMap<Article, ArticleEditViewModel>();
        }
    }
}