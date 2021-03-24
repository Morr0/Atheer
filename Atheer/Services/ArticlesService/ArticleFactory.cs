using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Atheer.Controllers.ArticleEdit.Models;
using Atheer.Extensions;
using Atheer.Models;
using AutoMapper;

[assembly: InternalsVisibleTo("AtheerTests")]
namespace Atheer.Services.ArticlesService
{
    public sealed class ArticleFactory
    {
        private readonly IMapper _mapper;

        public ArticleFactory(IMapper mapper)
        {
            _mapper = mapper;
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
    }
}