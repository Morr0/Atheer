using Amazon.DynamoDBv2.Model;
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
        public static Dictionary<string, AttributeValue> LastEvalKey(int lastEvaluatedKeyYear, string lastEvaluatedKeyTitle)
        {
            return new Dictionary<string, AttributeValue>
        {
            {nameof(BlogPost.CreatedYear).ToString(), new AttributeValue{ N = lastEvaluatedKeyYear.ToString() } },
            {nameof(BlogPost.TitleShrinked), new AttributeValue{ S = lastEvaluatedKeyTitle } }
        };
    }

        public static BlogPost Map(Dictionary<string, AttributeValue> dict)
        {
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
    }
}
