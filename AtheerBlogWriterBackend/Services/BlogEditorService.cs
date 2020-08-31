using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBlogWriterBackend.Exceptions;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
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
            newPost.LastUpdatedDate = DateTime.UtcNow.ToString();

            var forUpdating = GetUpdateValuesAndUpdateExpression(updateDTO);
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = CommonConstants.TABLE_NAME,
                Key = BlogPostExtensions.GetKey(newPost.CreatedYear, newPost.TitleShrinked),
                ExpressionAttributeValues = forUpdating.attributesValues,
                UpdateExpression = forUpdating.updateExpression,
                //ReturnValues = ReturnValue.ALL_NEW
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
            GetUpdateValuesAndUpdateExpression(BlogPostUpdateDTO updateDTO)
        {
            var updateAtts = new Dictionary<string, AttributeValue>();
            StringBuilder sb = new StringBuilder();
            var props = updateDTO.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name == nameof(BlogPost.CreatedYear) ||
                    prop.Name == nameof(BlogPost.TitleShrinked))
                    continue;

                if (prop.GetValue(updateDTO) != default)
                {
                    string dAttValName = $":{prop.Name}";
                    updateAtts.Add(dAttValName, BlogPostExtensions.AttributeVal(prop, updateDTO));
                    sb.Append($"SET {prop.Name} = {dAttValName} ");
                }
            }

            string updateExpression = sb.ToString().TrimEnd();
            return (updateAtts, updateExpression);
        }

        public async Task<bool> DeleteExistingPost(int year, string title)
        {
            var deleteItemRequest = new DeleteItemRequest
            {
                TableName = CommonConstants.TABLE_NAME,
                Key = BlogPostExtensions.GetKey(year, title),
                // To request the old item to be checked below if was there
                ReturnValues = ReturnValue.ALL_OLD
            };

            var deleteItemResponse = await _client.DeleteItemAsync(deleteItemRequest);
            // 0 -> the item was not in the table before attempt to delete
            if (deleteItemResponse.Attributes.Count == 0)
                return false;

            return true;
        }
    }
}
