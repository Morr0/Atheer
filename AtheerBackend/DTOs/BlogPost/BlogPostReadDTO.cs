using System;
using System.Collections.Generic;

namespace AtheerBackend.DTOs.BlogPost
{
    public class BlogPostReadDTO
    {
        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public List<string> Topics { get; set; }
        public bool Likeable { get; set; }
        public int Likes { get; set; }
        public bool Shareable { get; set; }
        public int Shares { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public bool Contactable { get; set; }
    }
}
