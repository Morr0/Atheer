using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AtheerCore.Models
{
    public class BlogPostUpdateDTO
    {
        [Required]
        [NotNull]
        public int CreatedYear { get; set; }
        [Required]
        [NotNull]
        public string TitleShrinked { get; set; }

        [NotNull]
        public string Description { get; set; }
        [NotNull]
        public string Content { get; set; }
        [NotNull]
        public string Topic { get; set; }
        [NotNull]
        public bool Draft { get; set; }
        [NotNull]
        public bool Unlisted { get; set; }
    }
}
