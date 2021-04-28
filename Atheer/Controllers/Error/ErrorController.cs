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

            return View("Error");
        }

        [HttpGet("/HandleCode")]
        public IActionResult HandleError([FromQuery] int code)
        {
            if (code == 404) return HTTP_404_Handler();
            if (code == 406) return HTTP_406_Handler();

            // TODO log it
            return Redirect("/");
        }

        [HttpGet("/NotFound")]
        public IActionResult HTTP_404_Handler()
        {
            return View("NotFound");
        }

        [HttpGet("/FormatInacceptable")]
        public IActionResult HTTP_406_Handler()
        {
            var reExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            var result = Json(new
            {
                Message = $"The requested path {reExecuteFeature?.OriginalPath} does not support the format/s in the Accept header"
            });
            result.StatusCode = 406;
            return result;
        }
    }
}