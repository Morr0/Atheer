using System.ComponentModel.DataAnnotations;

namespace AtheerCore.Models
{
    public class BlogPostUpdateDTO
    {
        [Required]
        public int CreatedYear { get; set; }
        [Required]
        public string TitleShrinked { get; set; }
        
        public string Description { get; set; }
        public string Content { get; set; }
        public string Topic { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
    }
}
