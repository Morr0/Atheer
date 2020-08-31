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

        public async Task UpdateExistingPost(BlogPostUpdateDTO updateDTO)
        {
            BlogPost oldPost = await GetPost(updateDTO.CreatedYear, updateDTO.TitleShrinked);
            if (oldPost == null)
                throw new BlogPostNotFoundException();

            BlogPost newPost = _mapper.Map<BlogPostUpdateDTO, BlogPost>(updateDTO, oldPost);
            newPost.LastUpdatedDate = DateTime.UtcNow.ToString();

            // Will start a DynamoDB transaction to remove old and add the new because:
            // 1- Can only update a single attribute in a record in a single class ( I want multiple attributes to be updated once)
            // 2- Cheaper to update multiple attributes at the same time
            var transactDeleteItem = new TransactWriteItem
            {
                Delete = new Delete
                {
                    TableName = CommonConstants.TABLE_NAME,
                    Key = BlogPostExtensions.GetKey(newPost.CreatedYear, newPost.TitleShrinked)
                },
            };

            var transactPutItem = new TransactWriteItem
            {
                Put = new Put
                {
                    TableName = CommonConstants.TABLE_NAME,
                    Item = BlogPostExtensions.Map(newPost)
                }
            };

            var transactWriteItemsRequest = new TransactWriteItemsRequest
            {
                TransactItems = new List<TransactWriteItem>
                {
                    transactDeleteItem,
                    transactPutItem
                }
            };
            // Transact
            var transactWriteItemsResponse = await _client.TransactWriteItemsAsync(transactWriteItemsRequest);
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
