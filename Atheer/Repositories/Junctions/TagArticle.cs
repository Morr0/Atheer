using Atheer.Models;

namespace Atheer.Repositories.Junctions
{
    public class TagArticle
    {
        public TagArticle()
        {
            
        }

        public TagArticle(Tag tag, Article article)
        {
            Tag = tag;
            Article = article;
        }
        
        public string TagId { get; set; }
        public Tag Tag { get; set; }
        
        public int ArticleCreatedYear { get; set; }
        public string ArticleTitleShrinked { get; set; }
        public Article Article { get; set; }
    }
}