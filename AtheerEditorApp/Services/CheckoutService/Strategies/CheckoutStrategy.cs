using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;
using AtheerEditorApp.Services.CheckoutService.Inputs;

namespace AtheerEditorApp.Services.CheckoutService.Strategies
{
    public abstract class CheckoutStrategy
    {
        protected AmazonDynamoDBClient _client;

        public CheckoutStrategy()
        {
            _client = new AmazonDynamoDBClient();
        }

        public abstract Task<bool> Checkout(BlogPost post, CheckoutInput input = null);

        
        // Checks that no post with the same primary key already exists
        protected async Task<bool> DoesPostAlreadyExist(BlogPost post)
        {
            return await GetPostElseNull(post.CreatedYear, post.TitleShrinked) != null;
        }

        public async Task<BlogPost> GetPostElseNull(int year, string titleShrinked)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = Singletons.ConstantsLoader.BlogPostTableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {nameof(BlogPost.CreatedYear), new AttributeValue{N = year.ToString()}},
                    {nameof(BlogPost.TitleShrinked), new AttributeValue{S = titleShrinked}}
                }
            };

            var getItemResponse = await _client.GetItemAsync(getItemRequest);
            if (getItemResponse.Item.Count == 0)
                return null;

            return DynamoToFromModelMapper<BlogPost>.Map(getItemResponse.Item);
        }
    }
}
