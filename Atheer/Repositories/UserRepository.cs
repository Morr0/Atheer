using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Utilities.Config.Models;
using Microsoft.Extensions.Options;

namespace Atheer.Repositories
{
    public class UserRepository
    {
        private readonly DynamoDbTables _config;
        private AmazonDynamoDBClient _client;

        public UserRepository(IOptions<DynamoDbTables> config)
        {
            _config = config.Value;
            _client = new AmazonDynamoDBClient();
        }

        public async Task<bool> Has(string email)
        {
            string emailAttName = nameof(User.Email);
            string emailAttValue = $":{nameof(User.Email)}";
            var request = new ScanRequest
            {
                TableName = _config.Users,
                ProjectionExpression = emailAttName,
                FilterExpression = $"{emailAttName} = {emailAttValue}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {emailAttValue, new AttributeValue{ S = email}}
                }
            };

            var response = await _client.ScanAsync(request).ConfigureAwait(false);
            return response.Items.Count > 0;
        }

        public Task Add(User user)
        {
            var request = new PutItemRequest
            {
                TableName = _config.Users,
                Item = DynamoToFromModelMapper<User>.Map(user)
            };

            return _client.PutItemAsync(request);
        }

        public async Task<User> Get(string id)
        {
            var request = new GetItemRequest
            {
                TableName = _config.Users,
                Key = DynamoToFromModelMapper<User>.GetUserKey(id)
            };

            var response = await _client.GetItemAsync(request).ConfigureAwait(false);
            return DynamoToFromModelMapper<User>.Map(response.Item);
        }
    }
}