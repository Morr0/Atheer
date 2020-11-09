using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;
using AtheerEditorApp.Services.CheckoutService.Inputs;

namespace AtheerEditorApp.Services.CheckoutService.Strategies
{
    public class RemovePostCheckoutStrategy : CheckoutStrategy
    {
        public override async Task<bool> Checkout(BlogPost post, CheckoutInput input = null)
        {
            var deleteItemRequest = new DeleteItemRequest
            {
                TableName = Singletons.ConstantsLoader.BlogPostTableName,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(post.CreatedYear, post.TitleShrinked),
            };

            var response = await _client.DeleteItemAsync(deleteItemRequest);

            return response.Attributes.Count > 0;
        }
    }
}