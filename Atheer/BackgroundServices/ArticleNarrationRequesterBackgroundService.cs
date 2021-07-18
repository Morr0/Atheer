using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Amazon.Polly;
using Amazon.Polly.Model;
using Atheer.Extensions;
using Atheer.Services.ArticlesService.Models;
using Atheer.Services.FileService;
using Atheer.Services.QueueService;
using Atheer.Utilities.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Atheer.BackgroundServices
{
    public class ArticleNarrationRequesterBackgroundService : BackgroundService
    {
        private readonly Channel<ArticleNarrationRequest> _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ILogger _logger;

        public ArticleNarrationRequesterBackgroundService(Channel<ArticleNarrationRequest> channel, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
        {
            _channel = channel;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = loggerFactory.CreateLogger(LoggingConstants.ArticleNarration);
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
            _logger.LogInformation("Received logging request for article id: {id} with content: {content}", 
                request.Id, request.Content);

            using var scope = _serviceScopeFactory.CreateScope();
            var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

            _logger.LogInformation("Calling Polly for article id: {id}", request.Id);
            var audio = await GetAudio(scope, request.Content).CAF();
            _logger.LogInformation("Finished from Polly for article id: {id}", request.Id);
            
            _logger.LogInformation("Persisting the audio as file for article id: {id}", request.Id);
            string url = await fileService.Add(FileUse.ArticleNarration, request.Id, "audio/mpeg", audio).CAF();
            _logger.LogInformation("Persisted article id: {id} narration at: {url}", request.Id, url);
            
            _logger.LogInformation("Almost finished with narrating article id: {id} will be persisting the change in the DB", request.Id);
            request.Callback(request.Id, url);
            _logger.LogInformation("Finished narration for article id: {id}", request.Id);

            await audio.DisposeAsync().CAF();
        }

        private async Task<Stream> GetAudio(IServiceScope scope, string content)
        {
            var pollyClient = scope.ServiceProvider.GetRequiredService<IAmazonPolly>();
            
            var pollyRequest = new SynthesizeSpeechRequest
            {
                Text = content,
                LanguageCode = LanguageCode.EnUS,
                VoiceId = VoiceId.Mathieu,
                OutputFormat = "mp3"
            };
            
            var pollyResponse = await pollyClient.SynthesizeSpeechAsync(pollyRequest).CAF();
            return pollyResponse.AudioStream;
        }
    }
}