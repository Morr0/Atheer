﻿using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;
using AtheerEditorApp.Exceptions;
using AtheerEditorApp.Services.CheckoutService.Inputs;

namespace AtheerEditorApp.Services.CheckoutService.Strategies
{
    internal class NewPostCheckoutStrategy : CheckoutStrategy
    {
        public override async Task<bool> Checkout(BlogPost post, CheckoutInput input = null)
        {
            if (await DoesPostAlreadyExist(post))
                throw new APostExistsWithSamePrimaryKeyException();
            
            PutItemRequest putItemRequest = new PutItemRequest
            {
                TableName = CommonConstants.BLOGPOST_TABLE,
                Item = BlogPostExtensions.Map(post)
            };

            var putItemResponse = await _client.PutItemAsync(putItemRequest);
            return true;
        }

    }
}
