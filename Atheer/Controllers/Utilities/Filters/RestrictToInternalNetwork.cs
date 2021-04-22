using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Atheer.Controllers.Utilities.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RestrictToInternalNetwork : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // TODO handle IPV6
            string ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
            bool isInternal = InternalIpv4Address(ipAddress);

            if (isInternal) await next().ConfigureAwait(false);
            else context.Result = new NotFoundResult();
        }
        
        internal static bool InternalIpv4Address(string address)
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
