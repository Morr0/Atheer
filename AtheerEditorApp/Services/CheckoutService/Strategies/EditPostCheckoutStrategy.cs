using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;
using AtheerEditorApp.Services.CheckoutService.Inputs;

namespace AtheerEditorApp.Services.CheckoutService.Strategies
{
    public sealed class EditPostCheckoutStrategy : CheckoutStrategy
    {
        #region updating directly using UpdateItem does not work due to a different reason each time
        //
        // public override async Task<bool> Checkout(BlogPost post)
        // {
        //     (string updateExpression, Dictionary<string, AttributeValue> values) = GetUpdatables(ref post);
        //     
        //     var request = new UpdateItemRequest
        //     {
        //         TableName = CommonConstants.BLOG_POSTS_TABLE_NAME,
        //         Key = BlogPostExtensions.GetKey(post.CreatedYear, post.TitleShrinked),
        //         ExpressionAttributeValues = values,
        //         UpdateExpression = updateExpression
        //     };
        //     
        //     var response = await _client.UpdateItemAsync(request);
        //     Console.WriteLine(response.Attributes.ToString());
        //     return response.Attributes.Count > 0;
        // }
        //
        // private (string, Dictionary<string, AttributeValue>) GetUpdatables(ref BlogPost post)
        // {
        //     var props = BlogPostExtensions.Map(post);
        //     var values = new Dictionary<string, AttributeValue>(props.Count - 2);
        //     StringBuilder sb = new StringBuilder(props.Count);
        //     sb.Append("SET ");
        //     
        //     foreach (var prop in props)
        //     {
        //         // if (name == nameof(BlogPost.TitleShrinked) || name == nameof(BlogPost.CreatedYear))
        //         //     continue;
        //
        //         string valueName = $":{prop.Key}";
        //         var value = GetNonEmptyAttributeValue(prop.Value);
        //         values.Add(valueName, prop.Value);
        //
        //         sb.Append($"{prop.Key} = {valueName}, ");
        //     }
        //
        //     sb.Remove(sb.Length - 2, 2);
        //     string updateExpression = sb.ToString();
        //
        //     return (updateExpression, values);
        // }
        //
        // private AttributeValue GetNonEmptyAttributeValue(AttributeValue propValue)
        // {
        //     bool has = false;
        //     foreach (var prop in propValue.GetType().GetProperties())
        //     {
        //         if (prop.GetValue(propValue) != default)
        //         {
        //             has = true;
        //             break;
        //         }
        //     }
        //
        //     return has ? propValue : new AttributeValue {N = "0"};
        // }
        #endregion

        #region this works by removing old item then putting new

        public override async Task<bool> Checkout(BlogPost post, CheckoutInput input = null)
        {
            var deleteRequest = new DeleteItemRequest
            {
                TableName = Singletons.ConstantsLoader.BlogPostTableName,
                Key = BlogPostExtensions.GetKey(post.CreatedYear, post.TitleShrinked),
                ReturnValues = ReturnValue.ALL_OLD
            };
            var deleteResponse = await _client.DeleteItemAsync(deleteRequest);

            Console.WriteLine(deleteResponse.Attributes.Count);
            BlogPost oldPost = BlogPostExtensions.Map(deleteResponse.Attributes);
            post.CreationDate = oldPost.CreationDate;
            post.TitleShrinked = oldPost.TitleShrinked;

            await Task.Delay(1500);
            
            var putItemRequest = new PutItemRequest
            {
                TableName = Singletons.ConstantsLoader.BlogPostTableName,
                Item = BlogPostExtensions.Map(post)
            };
            var putItemResponse = await _client.PutItemAsync(putItemRequest);
            
            return putItemResponse.Attributes.Count > 0;
        }

        #endregion
    }
}