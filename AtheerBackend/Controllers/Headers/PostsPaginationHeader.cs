using System.Diagnostics.CodeAnalysis;

namespace AtheerBackend.Controllers.Headers
{
    public class PostsPaginationHeader
    {
        [NotNull]
        public string X_AthBlog_Last_Year { get; set; }
        [NotNull]
        public string X_AthBlog_Last_Title { get; set; }
    }
}
