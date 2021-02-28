using Atheer.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetViewerUserId(this Controller @this)
        {
            return @this.User.FindFirst(AuthenticationController.CookieUserId)?.Value;
        }
    }
}