using System;
using System.Linq;
using Atheer.Controllers.ArticleEdit.Models;
using Atheer.Controllers.User.Models;
using Atheer.Models;
using Atheer.Services.OAuthService;
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
            CreateMap<OAuthUserInfo, User>();
        }

        private void TakeCareOfArticleToFromArticleEditVm()
        {
            CreateMap<ArticleEditViewModel, Article>();
            CreateMap<Article, ArticleEditViewModel>();
            CreateMap<AddArticleRequest, Article>();
            CreateMap<UpdateArticleViewModel, Article>()
                .ReverseMap();
        }
    }
}