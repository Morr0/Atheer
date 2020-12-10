using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using AtheerBackend.Models;

namespace AtheerBackend.Extensions
{
    public class DynamoToFromModelMapper<T> where T : new()
    {
        public static T Map(Dictionary<string, AttributeValue> dict)
        {
            if (dict.Count == 0)
                return default; // null

            T @object = new T();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (var prop in properties)
            {
                // Check if this property is modeled to avoid errors
                if (dict.ContainsKey(prop.Name))
                {
                    AttributeValue val = dict[prop.Name];

                    if (prop.PropertyType == typeof(string))
                        prop.SetValue(@object, val.S);
                    else if (prop.PropertyType == typeof(int))
                    {
                        try
                        {
                            prop.SetValue(@object, int.Parse(val.N));
                        } catch (ArgumentNullException)
                        {
                            prop.SetValue(@object, 0);
                        }
                    }
                    else if (prop.PropertyType == typeof(bool))
                        prop.SetValue(@object, val.BOOL);
                    else if (prop.PropertyType == typeof(List<string>))
                        prop.SetValue(@object, val.SS);
                    else
                        throw new Exception("The type from DynamoDB was not mapped.");
                        
                }
            }

            return @object;
        }

        public static object FromDynamoDB(object @object, PropertyInfo prop, AttributeValue val)
        {
            if (prop.PropertyType == typeof(string))
                prop.SetValue(@object, val.S);
            else if (prop.PropertyType == typeof(int))
            {
                try
                {
                    prop.SetValue(@object, int.Parse(val.N));
                } catch (ArgumentNullException)
                {
                    prop.SetValue(@object, 0);
                }
            }
            else if (prop.PropertyType == typeof(bool))
                prop.SetValue(@object, val.BOOL);
            else if (prop.PropertyType == typeof(List<string>))
                prop.SetValue(@object, val.SS);
            else
                throw new Exception("The type from DynamoDB was not mapped.");

            return @object;
        }

        public static Dictionary<string, AttributeValue> Map(T @object)
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            var dict = new Dictionary<string, AttributeValue>();

            foreach (var prop in props)
            {
                AttributeValue val = new AttributeValue();
                if (prop.PropertyType == typeof(int))
                    val.N = (((int)prop.GetValue(@object)).ToString()) ?? "0";
                else if (prop.PropertyType == typeof(string))
                    val.S = (prop.GetValue(@object) as string) ?? "";
                else if (prop.PropertyType == typeof(bool))
                    val.BOOL = (bool)prop.GetValue(@object);
                else if (prop.PropertyType == typeof(List<string>))
                    val.SS = (List<string>) prop.GetValue(@object) ?? new List<string>();
                else
                    throw new Exception("The type to DynamoDB was not mapped.");

                dict.Add(prop.Name, val);
            }

            return dict;
        }

        public static AttributeValue ToDynamoDB(object @object, PropertyInfo prop)
        {
            AttributeValue val = new AttributeValue();
            if (prop.PropertyType == typeof(int))
                val.N = ((int)prop.GetValue(@object)).ToString();
            else if (prop.PropertyType == typeof(string))
                val.S = (prop.GetValue(@object) as string) ?? "";
            else if (prop.PropertyType == typeof(bool))
                val.BOOL = (bool)prop.GetValue(@object);
            else if (prop.PropertyType == typeof(List<string>))
                val.SS = (List<string>) prop.GetValue(@object);
            else
                throw new Exception("The type to DynamoDB was not mapped.");

            return val;
        }

        // Get key
        public static Dictionary<string, AttributeValue> GetPostKey(int year, string titleShrinked)
        {
            return new Dictionary<string, AttributeValue>
            {
                {nameof(BlogPost.CreatedYear), new AttributeValue{N = year.ToString()} },
                {nameof(BlogPost.TitleShrinked), new AttributeValue{S = titleShrinked} }
            };
        }

        public static Dictionary<string, AttributeValue> GetContactKey(string id)
        {
            return new Dictionary<string, AttributeValue>
            {
                {nameof(Contact.Id), new AttributeValue(id)}
            };
        }
    }
}
