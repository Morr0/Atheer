using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBackend.Controllers.Headers;
using AtheerBackend.Extensions;
using AtheerCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtheerBackend.Services
{
    public class BlogRepository : IBlogRepository
    {
        public static string TABLE_NAME = "Atheer-Blog";

        private AmazonDynamoDBClient _client;

        public BlogRepository()
        {
            _client = new AmazonDynamoDBClient();
        }

        public async Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null)
        {
            var scanRequest = new ScanRequest
            {
                TableName = TABLE_NAME,
                Limit = amount,
            };
            // Query the last evaluated key if not null
            if (!paginationHeader.Empty())
            {
                scanRequest.ExclusiveStartKey = BlogPostExtensions.LastEvalKey(paginationHeader);
            }

            var scanResponse = await _client.ScanAsync(scanRequest);

            var response = new BlogRepositoryBlogResponse(scanResponse.Count);
            foreach (var item in scanResponse.Items)
            {
                response.Posts.Add(BlogPostExtensions.Map(item));
            }
            if (scanResponse.LastEvaluatedKey.Count > 0) 
            {
                Console.WriteLine("Not empty");
                response.PaginationHeader = BlogPostExtensions
                .PostsPaginationHeaderFromLastEvalKey(scanResponse.LastEvaluatedKey); 
            }

            return response;
        }

        public async Task<BlogPost> Get(int year, string title)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = TABLE_NAME,
                Key = new Dictionary<string, AttributeValue>
                {
                    {nameof(BlogPost.CreatedYear), new AttributeValue{N = year.ToString()} },
                    {nameof(BlogPost.TitleShrinked), new AttributeValue{S = title} }
                },
            };

            var getItemResponse = await _client.GetItemAsync(getItemRequest);
            return BlogPostExtensions.Map(getItemResponse.Item);
        }
    }
}
