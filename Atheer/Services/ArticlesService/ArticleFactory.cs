using System.Text;
using Atheer.Controllers.ArticleEdit.Models;
using Atheer.Models;
using Atheer.Services.ArticlesService.Models;
using Atheer.Services.Utilities.TimeService;
using AutoMapper;
using Markdig;

namespace Atheer.Services.ArticlesService
{
    public sealed class ArticleFactory
    {
        private readonly IMapper _mapper;
        private readonly MarkdownPipeline _markdownPipeline;
        private readonly ITimeService _timeService;

        public ArticleFactory(IMapper mapper, MarkdownPipeline markdownPipeline, ITimeService timeService)
        {
            _mapper = mapper;
            _markdownPipeline = markdownPipeline;
            _timeService = timeService;
        }

        public Article Create(ref ArticleEditViewModel articleViewModel, string userId)
        {
            var article = _mapper.Map<Article>(articleViewModel);

            article.AuthorId = userId;

            var now = _timeService.Get();
            article.CreatedYear = now.Year;
            article.TitleShrinked = GetShrinkedTitle(articleViewModel.Title);
            article.CreatedAt = now;
            
            return article;
        }
        
        public Article Create(AddArticleRequest request, string userId)
        {
            var article = _mapper.Map<Article>(request);

            article.AuthorId = userId;
            
            var currDate = _timeService.Get();
            article.CreatedYear = currDate.Year;
            article.TitleShrinked = GetShrinkedTitle(request.Title);
            article.CreatedAt = currDate;

            return article;
        }

        public void SetUpdated(Article article)
        {
            article.UpdatedAt = _timeService.Get();
        }

        private string GetShrinkedTitle(string title)
        {
            string[] splitTitle = title.Split();
            var sb = new StringBuilder(splitTitle.Length * 2);
            char separator = '-';
            for (var index = 0; index < splitTitle.Length; index++)
            {
                var t = splitTitle[index];
                sb.Append($"{t.ToLower()}");
                if ((index + 1) != splitTitle.Length) sb.Append(separator);
            }

            return sb.ToString();
        }

        public ArticleSeries CreateSeries(string author, string title, string description)
        {
            return new ArticleSeries
            {
                Title = title,
                Description = description,
                CreatedAt = _timeService.Get(),
                AuthorId = author
            };
        }

        public void FinishSeries(ArticleSeries series)
        {
            series.Finished = true;
        }

        public ArticleNarrationRequest CreateNarrationRequest(Article article)
        {
            string htmlContent = article.Content is null ? "" : Markdown.ToHtml(article.Content, _markdownPipeline);
            return new ArticleNarrationRequest
            {
                CreatedYear = article.CreatedYear,
                TitleShrinked = article.TitleShrinked,
                Content = htmlContent
            };
        }
    }
}