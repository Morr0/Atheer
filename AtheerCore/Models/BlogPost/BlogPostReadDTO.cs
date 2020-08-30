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
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public string Topic { get; set; }
        public int Likes { get; set; }
    }
}
