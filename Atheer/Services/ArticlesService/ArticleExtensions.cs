using Atheer.Models;

namespace Atheer.Services.ArticlesService
{
    public static class ArticleExtensions
    {
        public static bool HasAccessTo(this Article article, string userId, bool isAdmin = false)
        {
            if (isAdmin) return true;
            
            if (article.Draft)
            {
                if (article.AuthorId == userId) return true;

                return false;
            }
            
            return true;
        }
    }
}