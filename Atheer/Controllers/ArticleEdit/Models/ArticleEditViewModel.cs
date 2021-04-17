using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ArticleEdit.Models
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
        
        [Required] public string TagsAsString { get; set; }

        public bool Likeable { get; set; }
        public bool Shareable { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        
        public string AuthorId { get; set; }

        public string ScheduledSinceDate { get; set; }
        public string Schedule { get; set; }
        public string CreationDate { get; set; }
        public bool Unschedule { get; set; }
        public bool Scheduled { get; set; }

        public bool Narratable { get; set; }

    }
}