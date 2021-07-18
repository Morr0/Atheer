using System;

namespace Atheer.Services.ArticlesService.Models
{
    public class ArticleNarrationRequest
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public Action<string, string> Callback { get; set; }
    }
}