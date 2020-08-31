using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBlogWriterBackend.Exceptions;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtheerBlogWriterBackend.Services
{
    public class BlogEditorService : IBlogEditorService
    {
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
                TableName = CommonConstants.TABLE_NAME,
                Item = BlogPostExtensions.Map(post)
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

        public async Task<BlogPost> UpdateExistingPost(BlogPostUpdateDTO updateDTO)
        {
            BlogPost post = await GetPost(updateDTO.CreatedYear, updateDTO.TitleShrinked);
            if (post == null)
                throw new BlogPostNotFoundException();

            BlogPost newPost = _mapper.Map<BlogPostUpdateDTO, BlogPost>(updateDTO, post);

            var forUpdating = GetUpdateValuesAndUpdateExpression(post, newPost);
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = CommonConstants.TABLE_NAME,
                Key = BlogPostExtensions.GetKey(newPost.CreatedYear, newPost.TitleShrinked),
                ExpressionAttributeValues = forUpdating.attributesValues,
                UpdateExpression = forUpdating.updateExpression,
                ReturnValues = ReturnValue.ALL_NEW
            };

            var updateItemResponse = await _client.UpdateItemAsync(updateItemRequest);
            return BlogPostExtensions.Map(updateItemResponse.Attributes);
        }

        private async Task<BlogPost> GetPost(int year, string title)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = CommonConstants.TABLE_NAME,
                Key = BlogPostExtensions.GetKey(year, title),
            };

            var getItemResponse = await _client.GetItemAsync(getItemRequest);
            return BlogPostExtensions.Map(getItemResponse.Item);
        }

        private (Dictionary<string, AttributeValue> attributesValues, string updateExpression) 
            GetUpdateValuesAndUpdateExpression(BlogPost oldPost, BlogPost newPost)
        {

        }
    }
}
