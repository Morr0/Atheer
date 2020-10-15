using Amazon.DynamoDBv2.Model;
using AtheerCore.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using AtheerBackend.Services.BlogService;

namespace AtheerBackend.Extensions
{
    public static class PostsPaginationHeaderExtension
    {
        public static void AddHeaders(this PostsPaginationPrimaryKey header, IHeaderDictionary headerDict)
        {
            headerDict.Add(nameof(PostsPaginationPrimaryKey.X_AthBlog_Last_Year), header.X_AthBlog_Last_Year);
            headerDict.Add(nameof(PostsPaginationPrimaryKey.X_AthBlog_Last_Title), header.X_AthBlog_Last_Title);
        }

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
    }
}
