using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AtheerBlogWriterBackend.DTOs
{
    internal class BlogPostWriteDTO
    {
        [Required]
        [NotNull]
        public string Title { get; set; }
        [Required]
        [NotNull]
        public string Description { get; set; }
        [Required]
        [NotNull]
        public string Content { get; set; }
        [Required]
        [NotNull]
        public string Topic { get; set; }

    }
}