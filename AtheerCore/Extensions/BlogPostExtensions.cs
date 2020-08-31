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
                    {
                        try
                        {
                            prop.SetValue(post, int.Parse(val.N));
                        } catch (ArgumentNullException)
                        {
                            prop.SetValue(post, 0);
                        }
                    }
                    else if (prop.PropertyType == typeof(bool))
                        prop.SetValue(post, val.BOOL);
                    else
                        throw new Exception("The type from DynamoDB was not mapped.");
                        
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
                    val.N = (((int)prop.GetValue(post)).ToString()) ?? "0";
                else if (prop.PropertyType == typeof(string))
                    val.S = (prop.GetValue(post) as string) ?? "";
                else if (prop.PropertyType == typeof(bool))
                    val.BOOL = (bool)prop.GetValue(post);
                else
                    throw new Exception("The type to DynamoDB was not mapped.");

                dict.Add(prop.Name, val);
            }

            return dict;
        }

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
