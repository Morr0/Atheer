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
    public class ArticleRepository
    {
        private readonly DynamoDbTables _tables;
        private const string SortByLatestDatetimeFormat = "d/M/yyyy";
        
        private readonly AmazonDynamoDBClient _client;

        public ArticleRepository(IOptions<DynamoDbTables> tables)
        {
            _tables = tables.Value;
            _client = new AmazonDynamoDBClient();
        }

        #region Getting

        public async Task<Article> Get(ArticlePrimaryKey primaryKey)
        {
            var request = new GetItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<Article>.GetArticleKey(primaryKey.CreatedYear, primaryKey.TitleShrinked),
            };

            var response = await _client.GetItemAsync(request).ConfigureAwait(false);
            return DynamoToFromModelMapper<Article>.Map(response.Item);
        }

        public async Task<ArticleResponse> GetMany(int amount, ArticlePaginationPrimaryKey paginationHeader, 
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
                // filter, refer to AWS docs
                // Fetch only non-draft and listed posts and where there is no `TTL` attribute
                FilterExpression = $"attribute_not_exists({ttlNameSubstitute})"
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

        public async Task<ArticleResponse> GetMany(int year, int amount, 
        ArticlePaginationPrimaryKey paginationHeader, bool loadContentProperty)
        {
            string hashKey = nameof(Article.CreatedYear);
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
            PropertyInfo[] props = typeof(Article).GetProperties();
            StringBuilder sb = new StringBuilder(props.Length - 1);
            
            foreach (var prop in props)
            {
                if (prop.Name != nameof(Article.Content))
                    sb.Append($"{prop.Name},");
            }

            string expression = sb.ToString().TrimEnd(',');
            return expression;
        }

        private ArticleResponse ConstructResponse(int count,
            List<Dictionary<string, AttributeValue>> postsAttributes,
            Dictionary<string, AttributeValue> lastEvalKey)
        {
            var response = new ArticleResponse(count);
            foreach (var item in postsAttributes)
            {
                response.Articles.Add(DynamoToFromModelMapper<Article>.Map(item));
            }
            // if (lastEvalKey.Count > 0)
            // {
            //     response.PaginationHeader = PostsPaginationHeaderExtension
            //         .PostsPaginationHeaderFromLastEvalKey(lastEvalKey);
            // }

            response.Articles = SortDesc(response.Articles);

            return response;
        }

        private List<Article> SortDesc(List<Article> posts)
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

        public async Task<Article> Update(ArticlePrimaryKey key, Article newArticle)
        {
            var updatables = GetUpdateExpression(newArticle);
            var request = new UpdateItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<Article>.GetArticleKey(key.CreatedYear, key.TitleShrinked),
                UpdateExpression = updatables.updateString,
                ExpressionAttributeValues = updatables.values,
                ReturnValues = ReturnValue.ALL_NEW
            };

            var response = await _client.UpdateItemAsync(request).ConfigureAwait(false);
            return DynamoToFromModelMapper<Article>.Map(response.Attributes);
        }
        
        private (string updateString, Dictionary<string, AttributeValue> values) GetUpdateExpression(Article post)
        {
            var props = typeof(Article).GetProperties();
            var sb = new StringBuilder(props.Length * 2);
            sb.Append("SET "); // Check AWS docs for updating an item in DynamoDB
            var values = new Dictionary<string, AttributeValue>(props.Length * 2);

            foreach (var prop in props)
            {
                if (prop.Name != nameof(Article.CreatedYear) && prop.Name != nameof(Article.TitleShrinked))
                {
                    string valueName = $":{prop.Name}";
                    values.Add(valueName, DynamoToFromModelMapper<Article>.ToDynamoDB(post, prop));
                    
                    sb.Append($"{prop.Name} = {valueName},");
                }
            }

            sb.Remove(sb.Length - 1, 1);
            string updateString = sb.ToString();
            return (updateString, values);
        }

        public async Task<Article> IncrementSpecificPropertyIf(ArticlePrimaryKey primaryKey, string toIncrementPropertyName
            , string conditionPropertyName)
        {
            string toUpdatePropValue = $":{toIncrementPropertyName}";
            string updateConditionPropValue = $":{conditionPropertyName}";
            
            var request = new UpdateItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<Article>.GetArticleKey(primaryKey.CreatedYear, primaryKey.TitleShrinked),
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
                return DynamoToFromModelMapper<Article>.Map(response.Attributes);
            }
            catch (ConditionalCheckFailedException)
            {
                return null;
            }
        }

        #endregion

        public async Task<bool> GetFlag(string flag, ArticlePrimaryKey key)
        {
            var request = new GetItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<Article>.GetArticleKey(key.CreatedYear, key.TitleShrinked),
                ProjectionExpression = $"{flag}"
            };

            var response = await _client.GetItemAsync(request).ConfigureAwait(false);
            if (response.Item.Count == 0)
                return false;

            return response.Item[flag].BOOL;
        }

        public Task Delete(ArticlePrimaryKey key)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<Article>.GetArticleKey(key.CreatedYear, key.TitleShrinked)
            };
            
            return _client.DeleteItemAsync(request);
        }


        public async Task Add(Article post)
        {
            var request = new PutItemRequest
            {
                TableName = _tables.Posts,
                Item = DynamoToFromModelMapper<Article>.Map(post)
            };

            await _client.PutItemAsync(request).ConfigureAwait(false);
        }

        public async Task Update(Article post)
        {
            var sb = new StringBuilder().Append("SET ");
            var values = new Dictionary<string, AttributeValue>();
            foreach (var prop in post.GetType().GetProperties())
            {
                if (prop.Name == nameof(Article.TitleShrinked) || prop.Name == nameof(Article.CreatedYear))
                    continue;

                string propValueName = $":{prop.Name}";
                values.Add($"{propValueName}", DynamoToFromModelMapper<Article>.ToDynamoDB(post, prop));
                sb.Append($"{prop.Name} = {propValueName},");
            }

            string updateExpression = sb.ToString().TrimEnd(',');
            
            var request = new UpdateItemRequest
            {
                TableName = _tables.Posts,
                Key = DynamoToFromModelMapper<Article>.GetArticleKey(post.CreatedYear, post.TitleShrinked),
                UpdateExpression = updateExpression,
                ExpressionAttributeValues = values
            };

            await _client.UpdateItemAsync(request).ConfigureAwait(false);
        }
    }
}