using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Models;

namespace AtheerBackend.Services.CachedResultsService
{
    public interface ICachedResultsService
    {
        void Set(ref BlogPost post);
        BlogPost Get(ref BlogPostPrimaryKey primaryKey);
        void Invalidate(ref BlogPostPrimaryKey primaryKey);
    }
}