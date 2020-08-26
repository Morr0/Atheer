using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBackend.Controllers.Headers;
using AtheerBackend.Extensions;
using AtheerCore.Models;
using System.Collections.Generic;
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

        public async Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationHeader paginationHeader = null)
        {
            var scanRequest = new ScanRequest
            {
                TableName = TABLE_NAME,
                Limit = amount,
            };
            // Query the last evaluated key if not null
            if (paginationHeader != null)
            {
                scanRequest.ExclusiveStartKey = BlogPostExtensions.LastEvalKey(paginationHeader);
            }

            var scanResponse = await _client.ScanAsync(scanRequest);

            var response = new BlogRepositoryBlogResponse(scanResponse.Count);
            foreach (var item in scanResponse.Items)
            {
                response.Posts.Add(BlogPostExtensions.Map(item));
            }
            response.PaginationHeader = BlogPostExtensions.PostsPaginationHeaderFromLastEvalKey(scanResponse.LastEvaluatedKey);

            return response;
        }
    }
}
