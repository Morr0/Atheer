﻿using System.Collections.Generic;
using AtheerCore.Models;

namespace AtheerBackend.Controllers.Results
{
    /// <summary>
    /// The result sent back to client
    /// </summary>
    public class BlogPostsResult
    {
        public IEnumerable<BlogPostReadDTO> Posts { get; set; }
        
        public string X_AthBlog_Last_Year { get; set; }
        public string X_AthBlog_Last_Title { get; set; }
    }
}