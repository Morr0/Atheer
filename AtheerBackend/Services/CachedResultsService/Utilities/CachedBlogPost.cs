using System;
using AtheerCore.Models;

namespace AtheerBackend.Services.CachedResultsService.Utilities
{
    internal struct CachedBlogPost
    {
        // Seconds
        private const int CACHE_TIME = 60;

        private BlogPost _post;
        private DateTime _expiryTime;
        
        public CachedBlogPost(BlogPost post)
        {
            _post = post;
            _expiryTime = DateTime.UtcNow.AddSeconds(CACHE_TIME);
        }
        
        internal BlogPost Post => DateTime.UtcNow > _expiryTime ? null : _post;
    }
}