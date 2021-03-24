using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Atheer.Controllers.Utilities.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class OnlyVisibleToNetwork : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // TODO allow incoming request to only be from the same network otherwise return 404
            // TODO unit test it
        }

        public void OnActionExecuted(ActionExecutedContext context) {}
    }
}