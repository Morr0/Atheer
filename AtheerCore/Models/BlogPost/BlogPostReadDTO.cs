using System;

namespace AtheerCore.Models
{
    /// <summary>
    /// The blog post to read part
    /// </summary>
    public class BlogPostReadDTO
    {
        public int CreatedYear { get; set; } = DateTime.UtcNow.Year;
        public string TitleShrinked { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public string Topic { get; set; }
        public bool Likeable { get; set; }
        public int Likes { get; set; }
        public bool Shareable { get; set; }
        public int Shares { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
    }
}
