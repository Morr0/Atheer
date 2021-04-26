using Atheer.Controllers.Article;
using Atheer.Controllers.Article.Models;
using Atheer.Controllers.Article.Requests;
using Atheer.Controllers.User.Models;
using Atheer.Extensions;
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
            TakeCareOfArticlesToJsonFeed();
            
            // User
            CreateMap<RegisterViewModel, User>();
            CreateMap<User, UserSettingsViewModel>();
            CreateMap<UserSettingsUpdate, User>();
            CreateMap<OAuthUserInfo, User>();
        }

        private void TakeCareOfArticlesToJsonFeed()
        {
            CreateMap<Article, JsonFeedItem>()
                .ForMember(dst => dst.Id, cfg
                    => cfg.MapFrom(src => src.GetId()))
                .ForMember(dst => dst.Summary, cfg
                    => cfg.MapFrom(src => src.Description))
                .ForMember(dst => dst.DatePublished, cfg
                    => cfg.MapFrom(src => src.CreatedAt.GetString()))
                .ForMember(dst => dst.DateModified, cfg
                    => cfg.MapFrom(src => src.UpdatedAt.HasValue ? src.UpdatedAt.Value.GetString() : null))
                .ForMember(dst => dst.ContentHtml, cfg
                    => cfg.MapFrom(src => string.IsNullOrEmpty(src.Content)? "" : 
                        Markdig.Markdown.ToHtml(src.Content, Singletons.MarkdownPipeline)));
        }

        private void TakeCareOfArticleToFromArticleEditVm()
        {
            CreateMap<AddArticleRequest, Article>();
            CreateMap<UpdateArticleViewModel, Article>()
                .ReverseMap();
        }
    }
}