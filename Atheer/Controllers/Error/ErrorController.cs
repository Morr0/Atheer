using Atheer.Extensions;
using Atheer.Utilities.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Error
{
    public class ErrorController : Controller
    {
        private ILoggerFactory _loggerFactory;

        public ErrorController(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        
        // Exception handler
        [HttpGet("/Error")]
        public IActionResult ErrorHandler()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature is null || exceptionHandlerPathFeature.Path == "/Error") return NotFound();

            var logger = _loggerFactory.CreateLogger(LoggingConstants.UnhandledExceptionsCategory);
            logger.LogError(exceptionHandlerPathFeature.Error, "Exception thrown from the following path: {Path}", exceptionHandlerPathFeature.Path);

            return View("Error");
        }

        // THIS IS A SPECIAL REDIRECT FROM DENIED, to not expose denied to public in URL
        [HttpGet("/NotFound")]
        public IActionResult DeniedHandler([FromQuery] string path)
        {
            if (string.IsNullOrEmpty(path)) return View("NotFound");

            var logger = _loggerFactory.CreateLogger(LoggingConstants.ForbiddenPathCategory);
            string userId = this.GetViewerUserId();

            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("Attempt to hit forbidden endpoint: {Path}", path);
            }
            else
            {
                logger.LogWarning("Attempt to hit forbidden endpoint: {Path} by user: {UserId}", path, userId);
            }
            
            return View("NotFound");
        }

        [HttpGet("/HandleCode")]
        public IActionResult HandleError([FromQuery] int code)
        {
            var reExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            if (code == 404) return HTTP_404_Handler(reExecuteFeature);
            if (code == 406) return HTTP_406_Handler(reExecuteFeature);
            
            return Redirect("/");
        }
        
        public IActionResult HTTP_404_Handler(IStatusCodeReExecuteFeature reExecuteFeature)
        {
            var logger = _loggerFactory.CreateLogger(LoggingConstants.PageNotFoundCategory);
            logger.LogInformation("The following path: {Path} was not found", reExecuteFeature.OriginalPath);
            
            return View("NotFound");
        }
        
        public IActionResult HTTP_406_Handler(IStatusCodeReExecuteFeature reExecuteFeature)
        {
            var headersFeature = HttpContext.Features.Get<IHttpRequestFeature>();
            string acceptHeaderValues = headersFeature.Headers["Accept"].ToString();

            var result = Json(new
            {
                Message = $"The requested path {reExecuteFeature.OriginalPath} does not support the format/s in the Accept header: {acceptHeaderValues}"
            });
            result.StatusCode = 406;
            
            var logger = _loggerFactory.CreateLogger(LoggingConstants.ContentNotAcceptableCategory);
            logger.LogInformation("The following {Path} does not permit the Accept header's following: {AcceptHeaderValues}", 
                reExecuteFeature.OriginalPath, acceptHeaderValues);
            
            return result;
        }
    }
}