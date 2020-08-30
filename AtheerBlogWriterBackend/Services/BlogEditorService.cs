using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBlogWriterBackend.DTOs;
using AtheerCore.Models;
using AutoMapper;
using System;
using System.Threading.Tasks;

namespace AtheerBlogWriterBackend.Services
{
    public class BlogEditorService : IBlogEditorService
    {
        public static string TABLE_NAME = "Atheer-Blog";

        private IMapper _mapper;
        private AmazonDynamoDBClient _client;

        public BlogEditorService(IMapper mapper)
        {
            _mapper = mapper;
            _client = new AmazonDynamoDBClient();
        }

        public async Task<BlogPost> AddPost(BlogPostWriteDTO writeDTO)
        {
            BlogPost post = ConstructNewPost(writeDTO);

            // DynamoDB
            PutItemRequest putItemRequest = new PutItemRequest
            {
                TableName = TABLE_NAME,
                Item = BlogPostExtensions.Map(post),
                ReturnValues = ReturnValue.ALL_NEW
            };

            var putItemResponse = await _client.PutItemAsync(putItemRequest);
            return post;
        }

        private BlogPost ConstructNewPost(BlogPostWriteDTO writeDTO)
        {
            BlogPost post = _mapper.Map<BlogPost>(writeDTO);

            // Create first time metadata
            post.TitleShrinked = post.Title.TrimStart().TrimEnd()
                .ToLower().Replace(" ", "-");

            post.Id = Guid.NewGuid().ToString();
            post.CreationDate = post.LastUpdatedDate = 
                DateTime.UtcNow.ToString();

            return post;
        }
    }
}
