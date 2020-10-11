using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBackend.Controllers.Headers;
using AtheerBackend.Extensions;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtheerBackend.Services
{
    public class BlogRepository : IBlogRepository
    {
        private AmazonDynamoDBClient _client;

        public BlogRepository()
        {
            _client = new AmazonDynamoDBClient();
        }

        public async Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null)
        {

            var scanRequest = new ScanRequest
            {
                TableName = CommonConstants.BLOG_POSTS_TABLE_NAME,
                Limit = amount,
                
                // Fetch only non-draft and listed posts
                // Define the values looking for
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":false", new AttributeValue
                    {
                        BOOL = false
                    }}
                },
                // filter, refer to AWS docs
                FilterExpression = "Unlisted = :false AND Draft = :false "
            };
            // Query the last evaluated key if not null
            if (!paginationHeader.Empty())
            {
                scanRequest.ExclusiveStartKey = PostsPaginationHeaderExtension.LastEvalKey(paginationHeader);
            }

            var scanResponse = await _client.ScanAsync(scanRequest);

            var response = new BlogRepositoryBlogResponse(scanResponse.Count);
            foreach (var item in scanResponse.Items)
            {
                response.Posts.Add(BlogPostExtensions.Map(item));
            }
            if (scanResponse.LastEvaluatedKey.Count > 0) 
            {
                response.PaginationHeader = PostsPaginationHeaderExtension
                .PostsPaginationHeaderFromLastEvalKey(scanResponse.LastEvaluatedKey); 
            }

            return response;
        }

        public async Task<BlogRepositoryBlogResponse> GetByYear(int year, int amount, 
            PostsPaginationPrimaryKey paginationHeader = null)
        {
            string hashKey = nameof(BlogPost.CreatedYear);
            string vHashKey = $":{hashKey}";

            var queryRequest = new QueryRequest
            {
                TableName = CommonConstants.BLOG_POSTS_TABLE_NAME,
                KeyConditionExpression = $"{hashKey} = {vHashKey}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {vHashKey, new AttributeValue{N = year.ToString()} }
                },
                Limit = amount,
            };
            // Query the last evaluated key if not null
            if (!paginationHeader.Empty())
            {
                queryRequest.ExclusiveStartKey = PostsPaginationHeaderExtension.LastEvalKey(paginationHeader);
            }

            var queryResponse = await _client.QueryAsync(queryRequest);
            BlogRepositoryBlogResponse response = new BlogRepositoryBlogResponse(queryResponse.Count);
            foreach (var item in queryResponse.Items)
            {
                response.Posts.Add(BlogPostExtensions.Map(item));
            }
            if (queryResponse.LastEvaluatedKey.Count > 0)
            {
                response.PaginationHeader = PostsPaginationHeaderExtension
                .PostsPaginationHeaderFromLastEvalKey(queryResponse.LastEvaluatedKey);
            }

            return response;
        }

        public async Task<BlogPost> Get(int year, string title)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = CommonConstants.BLOG_POSTS_TABLE_NAME,
                Key = BlogPostExtensions.GetKey(year, title),
            };

            var getItemResponse = await _client.GetItemAsync(getItemRequest);
            return BlogPostExtensions.Map(getItemResponse.Item);
        }

        public async Task<BlogPost> Like(int year, string titleShrinked)
        {
            return await UpdateRecord(year, titleShrinked, UpdateBlogPostOperation.UpdateLikes);
        }

        public async Task<BlogPost> Share(int year, string titleShrinked)
        {
            return await UpdateRecord(year, titleShrinked, UpdateBlogPostOperation.UpdateShares);
        }

        private async Task<BlogPost> UpdateRecord(int year, string titleShrinked, UpdateBlogPostOperation operation
            , bool updateIfIsNotAllowed = false)
        {
            string toUpdatePropName = null;
            string updateConditonPropName = null;
            switch (operation)
            {
                default:
                case UpdateBlogPostOperation.UpdateLikes:
                    toUpdatePropName = nameof(BlogPost.Likes);
                    updateConditonPropName = nameof(BlogPost.Likeable);
                    break; 
                case UpdateBlogPostOperation.UpdateShares:
                    toUpdatePropName = nameof(BlogPost.Shares);
                    updateConditonPropName = nameof(BlogPost.Shareable);
                    break;
            }

            string toUpdatePropValue = $":{toUpdatePropName}";
            string updateConditonPropValue = $":{updateConditonPropName}";

            var updateItemRequest = new UpdateItemRequest
            {
                // Locating part
                TableName = CommonConstants.BLOG_POSTS_TABLE_NAME,
                Key = BlogPostExtensions.GetKey(year, titleShrinked),
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {toUpdatePropValue, new AttributeValue {N = 1.ToString()}},
                    {updateConditonPropValue, new AttributeValue {BOOL = true}}
                },
                // Add syntax
                UpdateExpression = $"ADD {toUpdatePropName} {toUpdatePropValue}",

                ReturnValues = ReturnValue.ALL_NEW
            };

            if (!updateIfIsNotAllowed)
            {
                updateItemRequest.ConditionExpression = $"{updateConditonPropName} = {updateConditonPropValue}";
            }
            
            // Update 
            try
            {
                var updateResponse = await _client.UpdateItemAsync(updateItemRequest);
                return BlogPostExtensions.Map(updateResponse.Attributes);
            }
            catch (ConditionalCheckFailedException)
            {
                return null;
            }
        }
    }
}
