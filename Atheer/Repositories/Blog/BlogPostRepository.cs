using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Atheer.Controllers.ViewModels;
using Atheer.Extensions;
using Atheer.Models;
using Atheer.Services;
using Atheer.Services.BlogService;
using Atheer.Utilities.Config.Models;
using Microsoft.Extensions.Options;

namespace Atheer.Repositories.Blog
{
    public class BlogPostRepository
    {
        private readonly DynamoDbTables _tables;
        private const string SortByLatestDatetimeFormat = "d/M/yyyy";
        
        private readonly AmazonDynamoDBClient _client;

        public BlogPostRepository(IOptions<DynamoDbTables> tables)
        {
            _tables = tables.Value;
            _client = new AmazonDynamoDBClient();
        }

        #region Getting

        public async Task<BlogPost> Get(BlogPostPrimaryKey primaryKey)
        {
            var request = new GetItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(primaryKey.CreatedYear, primaryKey.TitleShrinked),
            };

            var response = await _client.GetItemAsync(request).ConfigureAwait(false);
            return DynamoToFromModelMapper<BlogPost>.Map(response.Item);
        }

        public async Task<BlogRepositoryBlogResponse> GetMany(int amount, PostsPaginationPrimaryKey paginationHeader, 
            bool loadContentProperty)
        {
            string ttlNameSubstitute = $"#TTL";
            
            var request = new ScanRequest
            {
                TableName = _tables.Posts,
                Limit = amount,
                
                // Conditionals
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {ttlNameSubstitute, "TTL"}
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
            // if (!paginationHeader.Empty())
            // {
            //     request.ExclusiveStartKey = PostsPaginationHeaderExtension.LastEvalKey(paginationHeader);
            // }

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
                TableName = _tables.Posts,
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
            // if (!paginationHeader.Empty())
            // {
            //     request.ExclusiveStartKey = PostsPaginationHeaderExtension.LastEvalKey(paginationHeader);
            // }

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
            // if (lastEvalKey.Count > 0)
            // {
            //     response.PaginationHeader = PostsPaginationHeaderExtension
            //         .PostsPaginationHeaderFromLastEvalKey(lastEvalKey);
            // }

            response.Posts = SortDesc(response.Posts);

            return response;
        }

        private List<BlogPost> SortDesc(List<BlogPost> posts)
        {
            posts = posts.OrderByDescending(x => x.CreatedYear).ToList();
            posts.Sort((post1, post2) => SortDesc(post1.CreationDate, post2.CreationDate));
            return posts;
        }

        private int SortDesc(string creationDate1, string creationDate2)
        {
            if (string.IsNullOrEmpty(creationDate1) || string.IsNullOrEmpty(creationDate2))
                return default;

            creationDate1 = creationDate1.Split()[0];
            creationDate2 = creationDate2.Split()[0];

            try
            {
                var dt1 = DateTime.ParseExact(creationDate1, SortByLatestDatetimeFormat, CultureInfo.InvariantCulture);
                var dt2 = DateTime.ParseExact(creationDate2, SortByLatestDatetimeFormat, CultureInfo.InvariantCulture);

                return dt2.CompareTo(dt1);
            }
            catch (FormatException)
            {
                return default;
            }
            catch (InvalidOperationException)
            {
                return default;
            }
        }

        #endregion

        #region Updating

        public async Task<BlogPost> Update(BlogPostPrimaryKey key, BlogPost newBlogPost)
        {
            var updatables = GetUpdateExpression(newBlogPost);
            var request = new UpdateItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(key.CreatedYear, key.TitleShrinked),
                UpdateExpression = updatables.updateString,
                ExpressionAttributeValues = updatables.values,
                ReturnValues = ReturnValue.ALL_NEW
            };

            var response = await _client.UpdateItemAsync(request).ConfigureAwait(false);
            return DynamoToFromModelMapper<BlogPost>.Map(response.Attributes);
        }
        
        private (string updateString, Dictionary<string, AttributeValue> values) GetUpdateExpression(BlogPost post)
        {
            var props = typeof(BlogPost).GetProperties();
            var sb = new StringBuilder(props.Length * 2);
            sb.Append("SET "); // Check AWS docs for updating an item in DynamoDB
            var values = new Dictionary<string, AttributeValue>(props.Length * 2);

            foreach (var prop in props)
            {
                if (prop.Name != nameof(BlogPost.CreatedYear) && prop.Name != nameof(BlogPost.TitleShrinked))
                {
                    string valueName = $":{prop.Name}";
                    values.Add(valueName, DynamoToFromModelMapper<BlogPost>.ToDynamoDB(post, prop));
                    
                    sb.Append($"{prop.Name} = {valueName},");
                }
            }

            sb.Remove(sb.Length - 1, 1);
            string updateString = sb.ToString();
            return (updateString, values);
        }

        public async Task<BlogPost> IncrementSpecificPropertyIf(BlogPostPrimaryKey primaryKey, string toIncrementPropertyName
            , string conditionPropertyName)
        {
            string toUpdatePropValue = $":{toIncrementPropertyName}";
            string updateConditionPropValue = $":{conditionPropertyName}";
            
            var request = new UpdateItemRequest
            {
                TableName = _tables.Posts,
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

        public async Task<bool> GetFlag(string flag, BlogPostPrimaryKey key)
        {
            var request = new GetItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(key.CreatedYear, key.TitleShrinked),
                ProjectionExpression = $"{flag}"
            };

            var response = await _client.GetItemAsync(request).ConfigureAwait(false);
            if (response.Item.Count == 0)
                return false;

            return response.Item[flag].BOOL;
        }

        public Task Delete(BlogPostPrimaryKey key)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(key.CreatedYear, key.TitleShrinked)
            };
            
            return _client.DeleteItemAsync(request);
        }


        public async Task Add(BlogPost post)
        {
            var request = new PutItemRequest
            {
                TableName = _tables.Posts,
                Item = DynamoToFromModelMapper<BlogPost>.Map(post)
            };

            await _client.PutItemAsync(request).ConfigureAwait(false);
        }

        public async Task Update(BlogPost post)
        {
            var sb = new StringBuilder().Append("SET ");
            var values = new Dictionary<string, AttributeValue>();
            foreach (var prop in post.GetType().GetProperties())
            {
                if (prop.Name == nameof(BlogPost.TitleShrinked) || prop.Name == nameof(BlogPost.CreatedYear))
                    continue;

                string propValueName = $":{prop.Name}";
                values.Add($"{propValueName}", DynamoToFromModelMapper<BlogPost>.ToDynamoDB(post, prop));
                sb.Append($"{prop.Name} = {propValueName},");
            }

            string updateExpression = sb.ToString().TrimEnd(',');
            
            var request = new UpdateItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<BlogPost>.GetPostKey(post.CreatedYear, post.TitleShrinked),
                UpdateExpression = updateExpression,
                ExpressionAttributeValues = values
            };

            await _client.UpdateItemAsync(request).ConfigureAwait(false);
        }
    }
}