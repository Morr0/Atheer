using Amazon.DynamoDBv2;
using AtheerCore.Models;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;

namespace AtheerEditorApp.Services.Strategies
{
    internal abstract class CheckoutStrategy
    {
        protected AmazonDynamoDBClient _client;

        public CheckoutStrategy()
        {
            _client = new AmazonDynamoDBClient();
        }

        public abstract Task<bool> Checkout(BlogPost post);

        
        // Checks that no post with the same primary key already exists
        protected async Task<bool> DoesPostAlreadyExist(BlogPost post)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = CommonConstants.BLOG_POSTS_TABLE_NAME,
                Key = BlogPostExtensions.GetKey(post.CreatedYear, post.TitleShrinked),
            };

            var getItemResponse = await _client.GetItemAsync(getItemRequest);

            return getItemResponse.Item.Count > 0;
        }
    }
}
