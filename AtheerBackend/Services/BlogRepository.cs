using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using AtheerBackend.Extensions;
using AtheerCore.Models;
using System;
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

        public async Task<List<BlogPost>> Get(int amount)
        {
            var scanRequest = new ScanRequest
            {
                TableName = TABLE_NAME,
                Limit = amount,
            };

            var scanResponse = await _client.ScanAsync(scanRequest);

            List<BlogPost> posts = new List<BlogPost>(scanResponse.Items.Count);
            foreach (var item in scanResponse.Items)
            {
                posts.Add(BlogPostExtensions.Map(item));
            }

            return posts;
        }
    }
}
