using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBackend.DTOs;
using AtheerBackend.Repositories.Blog;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;

namespace AtheerBackend.Services.BlogService
{
    public class BlogPostService : IBlogPostService
    {
        private AmazonDynamoDBClient _client;

        private ConstantsLoader _constantsLoader;
        private BlogPostRepository _repository;

        public BlogPostService(ConstantsLoader constantsLoader, BlogPostRepository repository)
        {
            _client = new AmazonDynamoDBClient();
            _constantsLoader = constantsLoader;
            _repository = repository;
        }

        public Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null)
        {
            return _repository.GetMany(amount, paginationHeader);
        }

        public Task<BlogRepositoryBlogResponse> GetByYear(int year, int amount, 
            PostsPaginationPrimaryKey paginationHeader = null)
        {
            return _repository.GetMany(year, amount, paginationHeader);
        }

        public Task<BlogPost> Get(BlogPostPrimaryKey primaryKey)
        {
            return _repository.Get(primaryKey);
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
