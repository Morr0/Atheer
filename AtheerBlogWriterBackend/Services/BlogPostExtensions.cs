using Amazon.DynamoDBv2.Model;
using AtheerCore.Models;
using System.Collections.Generic;
using System.Reflection;

namespace AtheerBlogWriterBackend.Services
{
    internal class BlogPostExtensions
    {
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
    }
}