using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class ArticleEditViewModel
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
        public string TagsAsString { get; set; }

        public bool Likeable { get; set; }
        public bool Shareable { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }

        [Required, MinLength(1)]
        public string AuthorId { get; set; }
        [Required, MinLength(1)]
        public string NewAuthorId { get; set; }
    }
}