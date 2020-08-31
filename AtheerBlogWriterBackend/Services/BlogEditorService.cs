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
            return await AddPost(post);
        }

        private async Task<BlogPost> AddPost(BlogPost post)
        {
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

            post = _mapper.Map<BlogPostUpdateDTO, BlogPost>(updateDTO, post);
            post.LastUpdatedDate = DateTime.UtcNow.ToString();

            await DeleteExistingPost(post.CreatedYear, post.TitleShrinked);
            return await AddPost(post);
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

        // false -> there was not such item in the first place
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
