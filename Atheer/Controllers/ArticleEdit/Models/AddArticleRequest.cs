using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ArticleEdit.Models
{
    public class AddArticleRequest
    {
        [Required] public string Title { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string Content { get; set; }
        [Required] public string TagsAsString { get; set; }
        
        public bool Likeable { get; set; }
        public bool Shareable { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public bool Narratable { get; set; }
    }
}