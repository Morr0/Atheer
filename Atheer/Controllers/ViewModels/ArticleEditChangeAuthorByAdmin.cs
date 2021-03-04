using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class ArticleEditChangeAuthorByAdmin
    {
        public string AuthorId { get; set; }
        public string NewAuthorId { get; set; }

        public bool IsValid()
        {
            return !(string.IsNullOrEmpty(AuthorId) && string.IsNullOrEmpty(NewAuthorId));
        }
    }
}