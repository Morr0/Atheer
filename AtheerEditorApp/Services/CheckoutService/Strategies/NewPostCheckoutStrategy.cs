using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            var attributes = BlogPostExtensions.Map(post);
            if (input is NewArticleCheckoutSchedulingInput)
            {
                var scheduledInput = input as NewArticleCheckoutSchedulingInput;
                AddNegativeSignForPostIfScheduled(ref attributes);
                TakeCareOfTTLIfPostIsScheduled(ref scheduledInput, ref attributes);
            }

            PutItemRequest putItemRequest = new PutItemRequest
            {
                TableName = CommonConstants.BLOGPOST_TABLE,
                Item = attributes
            };

            var putItemResponse = await _client.PutItemAsync(putItemRequest);
            return true;
        }

        // Since a post that is scheduled will have it's created year start with -ve sign
        private void AddNegativeSignForPostIfScheduled(ref Dictionary<string, AttributeValue> attributes)
        {
            string createdYearStr = attributes[nameof(BlogPost.CreatedYear)].N;
            
            int negatedCreatedYear = int.Parse(createdYearStr) * -1;
            attributes[nameof(BlogPost.CreatedYear)] = new AttributeValue{N = negatedCreatedYear.ToString()};
        }

        private void TakeCareOfTTLIfPostIsScheduled(ref NewArticleCheckoutSchedulingInput schedulingInput,
            ref Dictionary<string, AttributeValue> attributes)
        {
            long ttl = ((DateTimeOffset)schedulingInput.ScheduleDate).ToUnixTimeSeconds();
            attributes.Add(CommonConstants.BLOGPOST_TABLE_TTL_ATTRIBUTE, new AttributeValue{N = ttl.ToString()});
        }
        
        

    }
}
