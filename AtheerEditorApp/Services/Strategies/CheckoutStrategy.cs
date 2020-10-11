using System.Collections.Generic;
using Amazon.DynamoDBv2;
using AtheerCore.Models;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;

namespace AtheerEditorApp.Services.Strategies
{
    public abstract class CheckoutStrategy
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
            return await GetPostElseNull(post.CreatedYear, post.TitleShrinked) != null;
        }

        public async Task<BlogPost> GetPostElseNull(int year, string titleShrinked)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = CommonConstants.BLOGPOST_TABLE,
                Key = new Dictionary<string, AttributeValue>
                {
                    {nameof(BlogPost.CreatedYear), new AttributeValue{N = year.ToString()}},
                    {nameof(BlogPost.TitleShrinked), new AttributeValue{S = titleShrinked}}
                }
            };

            var getItemResponse = await _client.GetItemAsync(getItemRequest);
            if (getItemResponse.Item.Count == 0)
                return null;

            return BlogPostExtensions.Map(getItemResponse.Item);
        }
    }
}
