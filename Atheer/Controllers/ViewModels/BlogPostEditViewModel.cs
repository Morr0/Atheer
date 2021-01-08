using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class BlogPostEditViewModel
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

        // TODO validate and take initialization off
        [Required] [MinLength(1)] public List<string> Topics { get; set; } = new List<string>
        {
            " "
        };

        public bool Likeable { get; set; }
        public bool Shareable { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public string LastUpdatedDate { get; set; }
    }
}