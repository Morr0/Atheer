using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Extensions;
using AtheerBackend.Models;
using AtheerBackend.Services.BlogService;
using AtheerBackend.Utilities;

namespace AtheerBackend.Repositories.Blog
{
    public class BlogPostRepository
    {
        private const string SortByLatestDatetimeFormat = "d/M/yyyy h:m:s tt";
        
        private readonly AtheerConfig _config;
        private readonly AmazonDynamoDBClient _client;

        public BlogPostRepository(AtheerConfig loader)
        {
            _config = loader;
            _client = new AmazonDynamoDBClient();
        }

        #region Getting

        public async Task<BlogPost> Get(BlogPostPrimaryKey primaryKey)
        {
            var request = new GetItemRequest
            {
                TableName = _config.PostsTable,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(primaryKey.CreatedYear, primaryKey.TitleShrinked),
            };

            var response = await _client.GetItemAsync(request).ConfigureAwait(false);
            return DynamoToFromModelMapper<BlogPost>.Map(response.Item);
        }

        public async Task<BlogRepositoryBlogResponse> GetMany(int amount, PostsPaginationPrimaryKey paginationHeader, 
            bool loadContentProperty)
        {
            string ttlNameSubstitute = $"#{AtheerConfig.TtlAttribute}";
            
            var request = new ScanRequest
            {
                TableName = _config.PostsTable,
                Limit = amount,
                
                // Conditionals
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {ttlNameSubstitute, AtheerConfig.TtlAttribute}
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
                TableName = _config.PostsTable,
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

            SortPostsByDayInMonth(response.Posts);

            return response;
        }

        private void SortPostsByDayInMonth(List<BlogPost> posts)
        {
            posts.Sort((post1, post2) => SortByDayInMonth(post1.CreationDate, post2.CreationDate));
        }

        private void SortPostsByDayInMonth(List<BareBlogPostReadDTO> posts)
        {
            posts.Sort((post1, post2) => SortByDayInMonth(post1.CreationDate, post2.CreationDate));
        }

        private int SortByDayInMonth(string creationDate1, string creationDate2)
        {
            try
            {
                var dt1 = DateTime.ParseExact(creationDate1, SortByLatestDatetimeFormat, null);
                var dt2 = DateTime.ParseExact(creationDate2, SortByLatestDatetimeFormat, null);

                return dt2.CompareTo(dt1);
            }
            catch (FormatException)
            {
                return default;
            }
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
                TableName = _config.PostsTable,
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

        public async Task<IEnumerable<BareBlogPostReadDTO>> GetManyBare()
        {
            var request = new ScanRequest
            {
                TableName = _config.PostsTable,
                ProjectionExpression = BareOnlyAttributeNames()
            };

            var response = await _client.ScanAsync(request);
            
            var posts = new List<BareBlogPostReadDTO>();
            foreach (var item in response.Items)
            {
                posts.Add(DynamoToFromModelMapper<BareBlogPostReadDTO>.Map(item));
            }
            
            SortPostsByDayInMonth(posts);
            return posts;
        }

        private string BareOnlyAttributeNames()
        {
            var props = typeof(BareBlogPostReadDTO).GetProperties();
            StringBuilder sb = new StringBuilder(props.Length);
            
            foreach (var prop in props)
            {
                sb.Append($"{prop.Name},");
            }

            string expression = sb.ToString().TrimEnd(',');
            return expression;
        }

        public async Task<bool> GetFlag(string flag, BlogPostPrimaryKey key)
        {
            var request = new GetItemRequest
            {
                TableName = _config.PostsTable,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(key.CreatedYear, key.TitleShrinked),
                ProjectionExpression = $"{flag}"
            };

            var response = await _client.GetItemAsync(request).ConfigureAwait(false);
            if (response.Item.Count == 0)
                return false;

            return response.Item[flag].BOOL;
        }

        public async Task Delete(BlogPostPrimaryKey key)
        {
            var request = new DeleteItemRequest
            {
                TableName = _config.PostsTable,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(key.CreatedYear, key.TitleShrinked)
            };
            
            var response = await _client.DeleteItemAsync(request).ConfigureAwait(false);
        }
    }
}