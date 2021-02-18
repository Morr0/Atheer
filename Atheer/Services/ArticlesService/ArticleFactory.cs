using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Atheer.Controllers.ViewModels;
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
            
            GetDate(articleViewModel.Schedule, out DateTime releaseDate, out bool scheduled, out string scheduledSinceDate);

            article.CreatedYear = releaseDate.Year;
            article.TitleShrinked = GetShrinkedTitle(articleViewModel.Title);
            article.CreationDate = releaseDate.GetString();
            article.Scheduled = scheduled;
            article.ScheduledSinceDate = scheduledSinceDate;

            return article;
        }

        internal void GetDate(string proposedDate, out DateTime releaseDate, out bool scheduled,
            out string scheduledSinceDate)
        {
            scheduled = false;
            scheduledSinceDate = "";
            var now = DateTime.UtcNow;

            bool parsedDate = DateTime.TryParseExact(proposedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out releaseDate);
            releaseDate = releaseDate.FirstTickOfDay();
            if (!parsedDate || releaseDate <= now) return;

            scheduled = true;
            scheduledSinceDate = now.GetString();
        }

        public void SetUpdated(ref Article article, bool unschedule)
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
            Console.WriteLine(article.ScheduledSinceDate);
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