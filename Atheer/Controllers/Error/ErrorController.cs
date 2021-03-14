using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers.Error
{
    public class ErrorController : Controller
    {
        // Exception handler
        [HttpGet("/Error")]
        public IActionResult ErrorHandler()
        {
            var exceptionHandlerPath = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            // TODO do something with exception
            
            TempData["Error"] = "An error has occured.";
            return Redirect("/");
        }

        [HttpGet("/NotFound")]
        public IActionResult NotFoundHandler()
        {
            TempData["Error"] = "The page you asked for does not exist.";
            return Redirect("/");
        }
    }
}