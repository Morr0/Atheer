namespace Atheer.Controllers.ViewModels
{
    public class StrippedArticleViewModel
    {
        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public string AuthorId { get; set; }
        public string CreationDate { get; set; }
    }
}