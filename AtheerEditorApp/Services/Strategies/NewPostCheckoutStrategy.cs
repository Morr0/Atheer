using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AtheerEditorApp.Services.Strategies
{
    internal class NewPostCheckoutStrategy : CheckoutStrategy
    {
        public NewPostCheckoutStrategy() : base() {}

        public override async Task<bool> Checkout(BlogPost post)
        {
            PutItemRequest putItemRequest = new PutItemRequest
            {
                TableName = CommonConstants.BLOG_POSTS_TABLE_NAME,
                Item = BlogPostExtensions.Map(post)
            };

            try
            {
                var putItemResponse = await _client.PutItemAsync(putItemRequest);
                return true;
            } catch (Exception)
            {
                throw;
            }
        }
    }
}
