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
            var article = _mapper.Map<Article>(articleViewModel);

            article.AuthorId = userId;
            
            var date = DateTime.UtcNow;
            GetDate(articleViewModel.Schedule, ref date, out bool scheduled, out DateTime scheduledSinceUtc);

            if (scheduled)
            {
                article.Scheduled = true;
                article.ScheduledSinceDate = scheduledSinceUtc.GetString();
            }

            article.CreatedYear = date.Year;
            article.TitleShrinked = GetShrinkedTitle(articleViewModel.Title);
            article.CreationDate = date.GetString();

            return article;
        }

        internal void GetDate(string proposedSchedule, ref DateTime date, out bool scheduled, out DateTime scheduledSince)
        {
            scheduled = false;
            scheduledSince = date;
            
            try
            {
                // Expected format dd-MM-yyyy
                var releaseDate = DateTime.ParseExact(proposedSchedule, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                
                // Assuming did not throw
                // Ignore if the proposed date in the past
                // int epochDay
                int releaseDateEpoch = (int) Math.Floor((releaseDate - new DateTime(1970, 1, 1)).TotalDays);
                int scheduledSinceDate = (int) Math.Floor((date - new DateTime(1970, 1, 1)).TotalDays);
                if (releaseDateEpoch > scheduledSinceDate) return;

                scheduled = true;
                date = releaseDate;
            }
            catch (Exception)
            {
                // Basically nothing
            }
        }

        // TODO update scheduling status
        public void SetUpdated(ref Article article)
        {
            article.LastUpdatedDate = DateTime.UtcNow.GetString();
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