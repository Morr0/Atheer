using AtheerBackend.DTOs;
using AtheerBackend.Services.CachedResultsService;
using AtheerCore.Models;
using Xunit;

namespace AtheerBackendTest
{
    public class CachedResultsServiceTest
    {
        [Fact]
        public void ShouldCacheABlogPostSuccessfully()
        {
            // Arrange
            BlogPostPrimaryKey primaryKey = new BlogPostPrimaryKey(2000, "hello");
            BlogPost post = new BlogPost
            {
                CreatedYear = primaryKey.CreatedYear,
                TitleShrinked = primaryKey.TitleShrinked,
                Title = "Hello"
            };
            
            ICachedResultsService cachedResultsService = new CachedResultsService();
            BlogPostPrimaryKey p = new BlogPostPrimaryKey(222, post.TitleShrinked);

            // Act
            cachedResultsService.Set(ref post);

            // Assert
            Assert.Equal(post, cachedResultsService.Get(ref primaryKey));
            Assert.Null(cachedResultsService.Get(ref p));
        }

        [Fact]
        public void ShouldInvalidateCacheSuccessfully()
        {
            // Arrange
            BlogPostPrimaryKey primaryKey = new BlogPostPrimaryKey(2000, "hello");
            BlogPost post = new BlogPost
            {
                CreatedYear = primaryKey.CreatedYear,
                TitleShrinked = primaryKey.TitleShrinked,
                Title = "Hello"
            };
            
            ICachedResultsService cachedResultsService = new CachedResultsService();
            BlogPostPrimaryKey p = new BlogPostPrimaryKey(post.CreatedYear, post.TitleShrinked);

            // Act
            cachedResultsService.Set(ref post);
            cachedResultsService.Invalidate(ref p);

            // Assert
            Assert.Null(cachedResultsService.Get(ref primaryKey));
        }
    }
}