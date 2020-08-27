using AtheerBackend.Controllers.Headers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtheerBackend.Extensions
{
    public static class PostsPaginationHeaderExtension
    {
        public static void AddHeaders(this PostsPaginationPrimaryKey header, IHeaderDictionary headerDict)
        {
            headerDict.Add(nameof(PostsPaginationPrimaryKey.X_AthBlog_Last_Year), header.X_AthBlog_Last_Year);
            headerDict.Add(nameof(PostsPaginationPrimaryKey.X_AthBlog_Last_Title), header.X_AthBlog_Last_Title);
        }
    }
}
