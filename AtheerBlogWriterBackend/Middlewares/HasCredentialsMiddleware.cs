using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using AtheerCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AtheerBlogWriterBackend.Middlewares
{
    public class HasCredentialsMiddleware
    {
        private readonly RequestDelegate _next;

        public HasCredentialsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.Headers.TryGetValue(CommonConstants.BLOG_POSTS_EDIT_KEY_NAME, out var __value);
            string requestSecret = __value.ToString();

            var getParameterRequest = new GetParameterRequest
            {
                Name = CommonConstants.BLOG_POSTS_EDIT_KEY_NAME
            };

            var client = new AmazonSimpleSystemsManagementClient();
            var getParameterResponse = await client.GetParameterAsync(getParameterRequest);

            if (getParameterResponse.Parameter.Value == requestSecret)
            {
                await _next(context);
            } else
            {
                context.Response.StatusCode = 403;
                return;
            }   
        }
    }
}
