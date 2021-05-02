using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Atheer.Middlewares
{
    public class UrlRewritingMiddleware
    {
        private readonly RequestDelegate _next;

        public UrlRewritingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value;
            RewriteToJsonFeedIfNeeded(context, path);

            await _next(context);
        }

        private void RewriteToJsonFeedIfNeeded(HttpContext context, string path)
        {
            if (path != "" && path != "/") return;
            
            bool acceptHeaderExists = context.Request.Headers.TryGetValue("Accept", out var acceptsHeaderValues);
            if (!acceptHeaderExists) return;

            if (acceptsHeaderValues.Contains("application/feed+json"))
            {
                context.Request.Path = "/json.feed";
            }
        }
    }
}