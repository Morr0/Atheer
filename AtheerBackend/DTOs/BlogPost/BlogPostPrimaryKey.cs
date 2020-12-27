using System.ComponentModel.DataAnnotations;

namespace AtheerBackend.DTOs.BlogPost
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