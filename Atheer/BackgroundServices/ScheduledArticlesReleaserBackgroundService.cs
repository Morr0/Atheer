using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atheer.Extensions;
using Atheer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Atheer.BackgroundServices
{
    public class ScheduledArticlesReleaserBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScheduledArticlesReleaserBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    await using var context = scope.ServiceProvider.GetService<Data>();
                    
                    await ProcessJob(context).ConfigureAwait(false);
                }

                // This must run on midnight
                await Task.Delay(DateTime.UtcNow.MillisecondsUntilNextDay(), stoppingToken).ConfigureAwait(false);
            } while (true);
        }

        private async Task ProcessJob(Data context)
        {
            // TODO Broken sync from UTC to local or other way around
            var today = DateTime.UtcNow;
            var articles = await context.Article
                .Where(x => x.Scheduled)
                .ToListAsync().ConfigureAwait(false);

            foreach (var article in articles)
            {
                var releaseDate = DateTime.Parse(article.CreationDate).FirstTickOfDay();
                if (!(today >= releaseDate)) continue;
                
                article.Scheduled = false;
                article.Draft = false;
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}