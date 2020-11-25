using System.Collections.Generic;
using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Models;
using AtheerBackend.Services.CachedResultsService.Utilities;

namespace AtheerBackend.Services.CachedResultsService
{
    public class CachedResultsService : ICachedResultsService
    {
        private Dictionary<BlogPostPrimaryKey, CachedBlogPost> _postsCached;

        public CachedResultsService()
        {
            _postsCached = new Dictionary<BlogPostPrimaryKey, CachedBlogPost>();
        }

        public void Set(ref BlogPost post)
        {
            BlogPostPrimaryKey key = new BlogPostPrimaryKey(post.CreatedYear, post.TitleShrinked);
            if (_postsCached.ContainsKey(key))
                _postsCached[key] = new CachedBlogPost(post);
            else 
                _postsCached.Add(key, new CachedBlogPost(post));
        }

        public BlogPost Get(ref BlogPostPrimaryKey primaryKey)
        {
            return _postsCached.ContainsKey(primaryKey) ? _postsCached[primaryKey].Post : null;
        }

        public void Invalidate(ref BlogPostPrimaryKey primaryKey)
        {
            if (!_postsCached.ContainsKey(primaryKey))
                return;

            _postsCached.Remove(primaryKey);
        }
    }
}