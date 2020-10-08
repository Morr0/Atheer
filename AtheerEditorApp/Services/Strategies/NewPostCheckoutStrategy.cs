using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AtheerEditorApp.Exceptions;

namespace AtheerEditorApp.Services.Strategies
{
    internal class NewPostCheckoutStrategy : CheckoutStrategy
    {
        public override async Task<bool> Checkout(BlogPost post)
        {
            if (await DoesPostAlreadyExist(post))
                throw new APostExistsWithSamePrimaryKeyException();
            
            PutItemRequest putItemRequest = new PutItemRequest
            {
                TableName = CommonConstants.BLOG_POSTS_TABLE_NAME,
                Item = BlogPostExtensions.Map(post)
            };

            var putItemResponse = await _client.PutItemAsync(putItemRequest);
            return true;
        }

    }
}
