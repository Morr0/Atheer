using Amazon.DynamoDBv2.Model;
using AtheerCore.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AtheerCore.Extensions
{
    public class BlogPostExtensions
    {

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

        public static Dictionary<string, AttributeValue> Map(BlogPost post)
        {
            PropertyInfo[] props = post.GetType().GetProperties();
            var dict = new Dictionary<string, AttributeValue>();

            foreach (var prop in props)
            {
                AttributeValue val = new AttributeValue();
                if (prop.PropertyType == typeof(int))
                    val.N = ((int)prop.GetValue(post)).ToString();
                else if (prop.PropertyType == typeof(string))
                    val.S = prop.GetValue(post) as string;

                dict.Add(prop.Name, val);
            }

            return dict;
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
