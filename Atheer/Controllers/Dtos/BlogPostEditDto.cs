﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Dtos
{
    public class BlogPostEditDto
    {
        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }
        [Required]
        [MinLength(3)]
        public string Title { get; set; }
        [Required]
        [MinLength(3)]
        public string Description { get; set; }
        [Required]
        [MinLength(3)]
        public string Content { get; set; }
        [Required]
        [MinLength(1)]
        public List<string> Topics { get; set; }
        public bool Likeable { get; set; }
        public bool Shareable { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public string LastUpdatedDate { get; set; }
    }
}