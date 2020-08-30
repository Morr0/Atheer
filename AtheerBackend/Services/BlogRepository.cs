﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
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
                TableName = CommonConstants.TABLE_NAME,
                Limit = amount,
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
                TableName = CommonConstants.TABLE_NAME,
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
                TableName = CommonConstants.TABLE_NAME,
                Key = BlogPostExtensions.GetKey(year, title),
            };

            var getItemResponse = await _client.GetItemAsync(getItemRequest);
            return BlogPostExtensions.Map(getItemResponse.Item);
        }

        public async Task<BlogPost> Like(int year, string title)
        {
            string likesAtt = nameof(BlogPost.Likes);
            // DynamoDB value name for use in updating
            string dNewLikes = ":newLikes";

            UpdateItemRequest updateItemRequest = new UpdateItemRequest
            {
                // Locating part
                TableName = CommonConstants.TABLE_NAME,
                Key = BlogPostExtensions.GetKey(year, title),
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { dNewLikes, new AttributeValue { N = 1.ToString() } }
                },
                // Add syntax
                UpdateExpression = $"ADD {likesAtt} {dNewLikes}",
                
                ReturnValues = ReturnValue.ALL_NEW
            };

            // Update 
            var updateResponse = await _client.UpdateItemAsync(updateItemRequest);
            if (updateResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return BlogPostExtensions.Map(updateResponse.Attributes);
        }
    }
}
