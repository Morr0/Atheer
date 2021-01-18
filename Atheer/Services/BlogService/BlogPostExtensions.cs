using Atheer.Models;

namespace Atheer.Services.BlogService
{
    public static class BlogPostExtensions
    {
        public static bool HasAccessTo(this BlogPost post, string userId, bool isAdmin = false)
        {
            if (isAdmin) return true;
            
            if (post.Draft)
            {
                if (post.AuthorId == userId) return true;

                return false;
            }
            
            return true;
        }
    }
}