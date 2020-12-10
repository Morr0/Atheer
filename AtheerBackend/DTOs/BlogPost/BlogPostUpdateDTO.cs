using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtheerBackend.DTOs.BlogPost
{
    public class BlogPostUpdateDTO
    {
        [Required] public int CreatedYear { get; set; }
        [Required] public string TitleShrinked { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public List<string> Topics { get; set; }
        public bool Likeable { get; set; } = true;
        public int Likes { get; set; }
        public bool Shareable { get; set; } = true;
        public int Shares { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public bool Contactable { get; set; }
        public bool Scheduled { get; set; }
    }
}