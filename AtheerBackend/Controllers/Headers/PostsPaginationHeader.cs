using System.Diagnostics.CodeAnalysis;

namespace AtheerBackend.Controllers.Headers
{
    public class PostsPaginationHeader
    {
        // The hash key
        [NotNull]
        public string X_AthBlog_Last_Year { get; set; }
        // The range key
        [NotNull]
        public string X_AthBlog_Last_Title { get; set; }

        public bool Empty() => string.IsNullOrEmpty(X_AthBlog_Last_Year) || string.IsNullOrEmpty(X_AthBlog_Last_Year);
    }
}
