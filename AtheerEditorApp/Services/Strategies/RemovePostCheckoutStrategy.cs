using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;

namespace AtheerEditorApp.Services.Strategies
{
    public class RemovePostCheckoutStrategy : CheckoutStrategy
    {
        public override async Task<bool> Checkout(BlogPost post)
        {
            var deleteItemRequest = new DeleteItemRequest
            {
                TableName = CommonConstants.BLOG_POSTS_TABLE_NAME,
                Key = BlogPostExtensions.GetKey(post.CreatedYear, post.TitleShrinked),
            };

            var response = await _client.DeleteItemAsync(deleteItemRequest);

            return response.Attributes.Count > 0;
        }
    }
}