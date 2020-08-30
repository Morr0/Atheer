using System.ComponentModel.DataAnnotations;

namespace AtheerCore.Models
{
    public class BlogPostWriteDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Topic { get; set; }
    }
}