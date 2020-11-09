﻿using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBackend.DTOs;
using AtheerBackend.Extensions;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;

namespace AtheerBackend.Services.BlogService
{
    public class BlogRepository : IBlogRepository
    {
        private AmazonDynamoDBClient _client;

        private ConstantsLoader _constantsLoader;

        public BlogRepository(ConstantsLoader constantsLoader)
        {
            _client = new AmazonDynamoDBClient();
            _constantsLoader = constantsLoader;
        }

        public async Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null)
        {
            string ttlNameSubstitute = $"#{_constantsLoader.BlogPostTableTTLAttribute}";
            
            var scanRequest = new ScanRequest
            {
                TableName = _constantsLoader.BlogPostTableName,
                Limit = amount,
                ProjectionExpression = GetAllExceptContentProperty(),
                
                // Conditionals
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {ttlNameSubstitute, _constantsLoader.BlogPostTableTTLAttribute}
                },
                // Define the values looking for
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":false", new AttributeValue
                    {
                        BOOL = false
                    }}
                },
                // filter, refer to AWS docs
                // Fetch only non-draft and listed posts and where there is no `TTL` attribute
                FilterExpression = $"Unlisted = :false AND Draft = :false AND attribute_not_exists({ttlNameSubstitute})"
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
                response.Posts.Add(DynamoToFromModelMapper<BlogPost>.Map(item));
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
                TableName = _constantsLoader.BlogPostTableName,
                Limit = amount,
                ProjectionExpression = GetAllExceptContentProperty(),
                
                KeyConditionExpression = $"{hashKey} = {vHashKey}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {vHashKey, new AttributeValue{N = year.ToString()} }
                },
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
                response.Posts.Add(DynamoToFromModelMapper<BlogPost>.Map(item));
            }
            if (queryResponse.LastEvaluatedKey.Count > 0)
            {
                response.PaginationHeader = PostsPaginationHeaderExtension
                .PostsPaginationHeaderFromLastEvalKey(queryResponse.LastEvaluatedKey);
            }

            return response;
        }

        // Formats this to use projection expression for all properties but the `content` to minimise
        // bandwidth usage. Although DynamoDB will not discount of the RCUs used.
        private string GetAllExceptContentProperty()
        {
            PropertyInfo[] props = typeof(BlogPost).GetProperties();
            StringBuilder sb = new StringBuilder(props.Length - 1);
            
            foreach (var prop in props)
            {
                if (prop.Name != nameof(BlogPost.Content))
                    sb.Append($"{prop.Name},");
            }

            string expression = sb.ToString().TrimEnd(',');
            return expression;
        }

        public async Task<BlogPost> Get(BlogPostPrimaryKey primaryKey)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = _constantsLoader.BlogPostTableName,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(primaryKey.CreatedYear, primaryKey.TitleShrinked),
            };

            var getItemResponse = await _client.GetItemAsync(getItemRequest);
            return DynamoToFromModelMapper<BlogPost>.Map(getItemResponse.Item);
        }

        public async Task<BlogPost> Like(BlogPostPrimaryKey primaryKey)
        {
            return await UpdateRecord(primaryKey, UpdateBlogPostOperation.UpdateLikes);
        }

        public async Task<BlogPost> Share(BlogPostPrimaryKey primaryKey)
        {
            return await UpdateRecord(primaryKey, UpdateBlogPostOperation.UpdateShares);
        }

        private async Task<BlogPost> UpdateRecord(BlogPostPrimaryKey primaryKey, UpdateBlogPostOperation operation
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
                TableName = _constantsLoader.BlogPostTableName,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(primaryKey.CreatedYear, primaryKey.TitleShrinked),
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
                return DynamoToFromModelMapper<BlogPost>.Map(updateResponse.Attributes);
            }
            catch (ConditionalCheckFailedException)
            {
                return null;
            }
        }
    }
}
