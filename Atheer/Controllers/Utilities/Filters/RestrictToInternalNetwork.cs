using System;
using System.Threading.Tasks;
using Atheer.Utilities.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Utilities.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RestrictToInternalNetwork : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(LoggingConstants.RestrictToInternalNetworkCategory);

            string ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            logger.LogInformation("Requesting a restricted action from IP: {Ip} for {RequestPath}",
                ipAddress, context.HttpContext.Request.Path.Value);

            bool isInternal = InternalIpAddress(ipAddress);
            if (isInternal)
            {
                logger.LogInformation("Successfully let internal IP {Ip} in for {RequestPath}",
                    ipAddress, context.HttpContext.Request.Path.Value);
                await next().ConfigureAwait(false);
            }
            else
            {
                logger.LogWarning("Denied IP {Ip} access to {RequestPath}",
                    ipAddress, context.HttpContext.Request.Path.Value);
                context.Result = new NotFoundResult();
            }
        }
        
        // TODO handle IPV6
        internal static bool InternalIpAddress(string address)
        {
            if (address == "127.0.0.1") return true;
            
            string[] splitAddr = address.Split('.');
            if (splitAddr[0] == "10") return true;
            if (splitAddr[0] == "172")
            {
                byte sec = byte.Parse(splitAddr[1]);
                if (sec >= 16 && sec <= 31) return true;
            }

            return splitAddr[0] == "192" && splitAddr[1] == "168";
        }
    }
}
