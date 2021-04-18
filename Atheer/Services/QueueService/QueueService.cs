using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Atheer.Services.QueueService.Exceptions;
using Atheer.Utilities.Config.Models;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Atheer.Services.QueueService
{
    public class QueueService : IQueueService
    {
        private readonly IOptions<SQS> _config;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IAmazonSQS _client;

        public QueueService(IOptions<SQS> config)
        {
            _config = config;
            _retryPolicy = Policy.Handle<AmazonSQSException>().RetryAsync(3);
            _client = new AmazonSQSClient();
        }
        
        public async Task Queue(QueueType type, QueueMessage message)
        {
            string url = GetQueueUrl(type);     
            
            var request = new SendMessageRequest
            {
                QueueUrl = url,
                MessageBody = message.MessageText,
            };
            await _retryPolicy.ExecuteAsync(() => _client.SendMessageAsync(request)).ConfigureAwait(false);
        }

        private string GetQueueUrl(QueueType type)
        {
            switch (type)
            {
                case QueueType.ArticleVoice:
                    return _config.Value.TranscriptionQueueUrl;
                default:
                    throw new QueueNotFoundException();
            }
        }
    }
}