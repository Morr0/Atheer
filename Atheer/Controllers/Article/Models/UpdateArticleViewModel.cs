using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Article.Models
{
    public class UpdateArticleViewModel
    {
        [Required] public string Title { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string Content { get; set; }
        [Required] public string TagsAsString { get; set; }
        public int? SeriesId { get; set; }
        
        public bool Likeable { get; set; }
        public bool Shareable { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public bool Narratable { get; set; }
    }
}