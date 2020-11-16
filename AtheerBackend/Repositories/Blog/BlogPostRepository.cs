using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBackend.DTOs;
using AtheerBackend.Extensions;
using AtheerBackend.Services.BlogService;
using AtheerCore;
using AtheerCore.Extensions;
using AtheerCore.Models;

namespace AtheerBackend.Repositories.Blog
{
    public class BlogPostRepository
    {
        private readonly ConstantsLoader _constantsLoader;
        private readonly AmazonDynamoDBClient _client;

        public BlogPostRepository(ConstantsLoader loader)
        {
            _constantsLoader = loader;
            _client = new AmazonDynamoDBClient();
        }

        #region Getting

        public async Task<BlogPost> Get(BlogPostPrimaryKey primaryKey)
        {
            var request = new GetItemRequest
            {
                TableName = _constantsLoader.BlogPostTableName,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(primaryKey.CreatedYear, primaryKey.TitleShrinked),
            };

            var response = await _client.GetItemAsync(request).ConfigureAwait(false);
            return DynamoToFromModelMapper<BlogPost>.Map(response.Item);
        }

        public async Task<BlogRepositoryBlogResponse> GetMany(int amount, PostsPaginationPrimaryKey paginationHeader, 
            bool loadContentProperty)
        {
            string ttlNameSubstitute = $"#{_constantsLoader.BlogPostTableTTLAttribute}";
            
            var request = new ScanRequest
            {
                TableName = _constantsLoader.BlogPostTableName,
                Limit = amount,
                
                // Conditionals
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {ttlNameSubstitute, _constantsLoader.BlogPostTableTTLAttribute}
                },
                // Define the values looking for
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":false", new AttributeValue
                    {
                        BOOL = false
                    }}
                },
                // filter, refer to AWS docs
                // Fetch only non-draft and listed posts and where there is no `TTL` attribute
                FilterExpression = $"Unlisted = :false AND Draft = :false AND attribute_not_exists({ttlNameSubstitute})"
            };
            
            if (!loadContentProperty)
            {
                request.ProjectionExpression = GetAllExceptContentProperty();
            }
            
            // Query the last evaluated key if not null
            if (!paginationHeader.Empty())
            {
                request.ExclusiveStartKey = PostsPaginationHeaderExtension.LastEvalKey(paginationHeader);
            }

            var response = await _client.ScanAsync(request).ConfigureAwait(false);

            return ConstructResponse(response.Count, response.Items, response.LastEvaluatedKey);
        }

        public async Task<BlogRepositoryBlogResponse> GetMany(int year, int amount, 
        PostsPaginationPrimaryKey paginationHeader, bool loadContentProperty)
        {
            string hashKey = nameof(BlogPost.CreatedYear);
            string vHashKey = $":{hashKey}";

            var request = new QueryRequest
            {
                TableName = _constantsLoader.BlogPostTableName,
                Limit = amount,

                KeyConditionExpression = $"{hashKey} = {vHashKey}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {vHashKey, new AttributeValue{N = year.ToString()} }
                },
            };

            if (!loadContentProperty)
            {
                request.ProjectionExpression = GetAllExceptContentProperty();
            }
            
            // Query the last evaluated key if not null
            if (!paginationHeader.Empty())
            {
                request.ExclusiveStartKey = PostsPaginationHeaderExtension.LastEvalKey(paginationHeader);
            }

            var response = await _client.QueryAsync(request).ConfigureAwait(false);

            return ConstructResponse(response.Count, response.Items, response.LastEvaluatedKey);
        }

        // Formats this to use projection expression for all properties but the `content` to minimise
        // bandwidth usage. Although DynamoDB will not discount of the RCUs used.
        private string GetAllExceptContentProperty()
        {
            PropertyInfo[] props = typeof(BlogPost).GetProperties();
            StringBuilder sb = new StringBuilder(props.Length - 1);
            
            foreach (var prop in props)
            {
                if (prop.Name != nameof(BlogPost.Content))
                    sb.Append($"{prop.Name},");
            }

            string expression = sb.ToString().TrimEnd(',');
            return expression;
        }

        private BlogRepositoryBlogResponse ConstructResponse(int count,
            List<Dictionary<string, AttributeValue>> postsAttributes,
            Dictionary<string, AttributeValue> lastEvalKey)
        {
            var response = new BlogRepositoryBlogResponse(count);
            foreach (var item in postsAttributes)
            {
                response.Posts.Add(DynamoToFromModelMapper<BlogPost>.Map(item));
            }
            if (lastEvalKey.Count > 0)
            {
                response.PaginationHeader = PostsPaginationHeaderExtension
                    .PostsPaginationHeaderFromLastEvalKey(lastEvalKey);
            }

            SortPostsByDayInMonth(ref response);

            return response;
        }

        private void SortPostsByDayInMonth(ref BlogRepositoryBlogResponse response)
        {
            response.Posts.Sort((post1, post2) =>
            {
                DateTime dt1 = DateTime.Parse(post1.CreationDate);
                DateTime dt2 = DateTime.Parse(post2.CreationDate);

                return dt2.CompareTo(dt1);
            });
        }

        #endregion

        #region Updating

        public async Task<BlogPost> IncrementSpecificPropertyIf(BlogPostPrimaryKey primaryKey, string toIncrementPropertyName
            , string conditionPropertyName)
        {
            string toUpdatePropValue = $":{toIncrementPropertyName}";
            string updateConditionPropValue = $":{conditionPropertyName}";
            
            var request = new UpdateItemRequest
            {
                TableName = _constantsLoader.BlogPostTableName,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(primaryKey.CreatedYear, primaryKey.TitleShrinked),
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {toUpdatePropValue, new AttributeValue {N = 1.ToString()}},
                    {updateConditionPropValue, new AttributeValue {BOOL = true}}
                },
                UpdateExpression = $"ADD {toIncrementPropertyName} {toUpdatePropValue}",
                
                ConditionExpression = $"attribute_exists({conditionPropertyName}) AND {conditionPropertyName} = {updateConditionPropValue}",

                ReturnValues = ReturnValue.ALL_NEW
            };

            try
            {
                var response = await _client.UpdateItemAsync(request);
                return DynamoToFromModelMapper<BlogPost>.Map(response.Attributes);
            }
            catch (ConditionalCheckFailedException)
            {
                return null;
            }
        }

        #endregion
    }
}