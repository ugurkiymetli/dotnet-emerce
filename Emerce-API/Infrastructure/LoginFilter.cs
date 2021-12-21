using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Emerce_API.Infrastructure
{
    public class LoginFilter : Attribute, IActionFilter
    {
        public void OnActionExecuted( ActionExecutedContext context )
        {
            return;
        }

        public void OnActionExecuting( ActionExecutingContext context )
        {
            var redisCache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
            var loginUserFromCache = redisCache.Get("Login");
            if ( loginUserFromCache is null )
            {
                context.Result = new BadRequestObjectResult("Please login!");
            }
            return;
        }
    }
}