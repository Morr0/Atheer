﻿using System.ComponentModel.DataAnnotations;

namespace Atheer.Services
{
    public class BlogPostPrimaryKey
    {
        public BlogPostPrimaryKey()
        {
            
        }

        public BlogPostPrimaryKey(int createdYear, string titleShrinked)
        {
            CreatedYear = createdYear;
            TitleShrinked = titleShrinked;
        }
        
        [Required]
        public int CreatedYear { get; set; }
        [Required]
        public string TitleShrinked { get; set; }
    }
}