using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Atheer.Controllers.ArticleEdit.Models;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services.ArticlesService.Models;
using AutoMapper;
using Markdig;

namespace Atheer.Services.ArticlesService
{
    public sealed class ArticleFactory
    {
        private readonly IMapper _mapper;
        private readonly MarkdownPipeline _markdownPipeline;

        public ArticleFactory(IMapper mapper, MarkdownPipeline markdownPipeline)
        {
            _mapper = mapper;
            _markdownPipeline = markdownPipeline;
        }

        public Article Create(ref ArticleEditViewModel articleViewModel, string userId)
        {
            // TERMS:
            // Release date = creation date when unscheduled or when the schedule ends
            
            var article = _mapper.Map<Article>(articleViewModel);

            article.AuthorId = userId;
            
            GetDate(articleViewModel.Schedule, out var releaseDate, out bool scheduled, out string scheduledSinceDate);

            article.CreatedYear = releaseDate.Year;
            article.TitleShrinked = GetShrinkedTitle(articleViewModel.Title);
            article.CreationDate = releaseDate.GetString();
            article.Scheduled = scheduled;
            article.ScheduledSinceDate = scheduled ? scheduledSinceDate : string.Empty;

            return article;
        }

        internal void GetDate(string proposedDate, out DateTime releaseDate, out bool scheduled,
            out string scheduledSinceDate)
        {
            scheduled = false;
            scheduledSinceDate = "";
            var now = DateTime.UtcNow;
            releaseDate = now;

            bool parsedDate = DateTime.TryParseExact(proposedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out releaseDate);

            if (!parsedDate || releaseDate < now)
            {
                releaseDate = now;
                return;
            }
            
            releaseDate = releaseDate.FirstTickOfDay();

            scheduled = true;
            scheduledSinceDate = now.GetString();
        }

        public void SetUpdated(Article article, bool unschedule)
        {
            var now = DateTime.UtcNow;
            article.LastUpdatedDate = now.GetString();
            
            // If is scheduled and still not yet released
            if (unschedule && article.Scheduled)
            {
                Unschedule(article, now);
            }
        }

        public void Unschedule(Article article, DateTime now)
        {
            var releaseDate = DateTime.Parse(article.CreationDate);
            var scheduledSince = DateTime.Parse(article.ScheduledSinceDate);
            // Not Between
            if (!(scheduledSince <= now && now < releaseDate)) return;

            article.CreationDate = now.GetString();
            article.Scheduled = false;
            // Now it is released of scheduling
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
                DateCreated = DateTime.UtcNow.GetString(),
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