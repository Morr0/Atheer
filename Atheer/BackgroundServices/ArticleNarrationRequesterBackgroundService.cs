using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Atheer.Services.ArticlesService.Models;
using Atheer.Services.QueueService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Atheer.BackgroundServices
{
    public class ArticleNarrationRequesterBackgroundService : BackgroundService
    {
        private readonly Channel<ArticleNarrationRequest> _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ArticleNarrationRequesterBackgroundService(Channel<ArticleNarrationRequest> channel, IServiceScopeFactory serviceScopeFactory)
        {
            _channel = channel;
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                if (stoppingToken.IsCancellationRequested) break;

                await _channel.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false);
                var request = await _channel.Reader.ReadAsync().ConfigureAwait(false);

                await RequestNarration(request).ConfigureAwait(false);
            }
        }

        private async Task RequestNarration(ArticleNarrationRequest request)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();

            string messageText = await Task.Run(() => JsonSerializer.Serialize(request)).ConfigureAwait(false);
            
            await queueService.Queue(QueueType.ArticleVoice, new QueueMessage
            {
                MessageText = messageText
            }).ConfigureAwait(false);
        }
    }
}