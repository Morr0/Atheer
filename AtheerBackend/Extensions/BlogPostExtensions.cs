using Amazon.DynamoDBv2.Model;
using AtheerBackend.Controllers.Headers;
using AtheerCore.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AtheerBackend.Extensions
{
    public class BlogPostExtensions
    {
        // Static primary key for later use
        // Reason for it is to not initialize a new primary each time needed
        public static Dictionary<string, AttributeValue> LastEvalKey(PostsPaginationPrimaryKey paginationHeader)
        {
            return new Dictionary<string, AttributeValue>
            {
                {nameof(BlogPost.CreatedYear).ToString(), new AttributeValue{ N = paginationHeader.X_AthBlog_Last_Year.ToString() } },
                {nameof(BlogPost.TitleShrinked), new AttributeValue{ S = paginationHeader.X_AthBlog_Last_Title } }
            };
        }

        public static PostsPaginationPrimaryKey PostsPaginationHeaderFromLastEvalKey(Dictionary<string, AttributeValue> dict)
        {
            return new PostsPaginationPrimaryKey
            {
                X_AthBlog_Last_Year = dict[nameof(BlogPost.CreatedYear)].N,
                X_AthBlog_Last_Title = dict[nameof(BlogPost.TitleShrinked)].S
            };
        }

        public static BlogPost Map(Dictionary<string, AttributeValue> dict)
        {
            if (dict.Count == 0)
                return null;

            BlogPost post = new BlogPost();
            Type type = post.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var prop in properties)
            {
                // Check if this property is modeled to avoid errors
                if (dict.ContainsKey(prop.Name))
                {
                    AttributeValue val = dict[prop.Name];

                    if (prop.PropertyType == typeof(string))
                        prop.SetValue(post, val.S);
                    else if (prop.PropertyType == typeof(int))
                        prop.SetValue(post, int.Parse(val.N));
                    else
                        throw new Exception("The type in DynamoDB was not mapped.");
                        
                }
            }

            return post;
        }

        // HELPERS

        // Get key
        // To reduce code duplication
        public static Dictionary<string, AttributeValue> GetKey(int year, string titleShrinked)
        {
            return new Dictionary<string, AttributeValue>
            {
                {nameof(BlogPost.CreatedYear), new AttributeValue{N = year.ToString()} },
                {nameof(BlogPost.TitleShrinked), new AttributeValue{S = titleShrinked} }
            };
        }
    }
}
