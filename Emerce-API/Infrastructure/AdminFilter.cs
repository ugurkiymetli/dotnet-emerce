using Emerce_Model.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Emerce_API.Infrastructure
{
    public class AdminFilter : Attribute, IActionFilter
    {
        //private readonly IMemoryCache memoryCache;

        //public AdminFilter( IMemoryCache _memoryCache )
        //{
        //    memoryCache = _memoryCache;
        //}
        public void OnActionExecuted( ActionExecutedContext context )
        {
            return;
        }

        public void OnActionExecuting( ActionExecutingContext context )
        {
            var memoryCache = context.HttpContext.RequestServices.GetService<IMemoryCache>();
            memoryCache.TryGetValue("Login", out UserViewModel loginUser);
            if ( !loginUser.IsAdmin )
            {
                context.Result = new BadRequestObjectResult("Please login as admin!");
            }
            return;
            //if ( !memoryCache.TryGetValue("Login", out UserViewModel loginUser)
            //{
            //    context.Result = new BadRequestObjectResult("Please login!");
            //}
            //if ( loginUser.IsAdmin )
            //{
            //    return;
            //}
            //else { context.Result = new BadRequestObjectResult("Please login as admin!"); }
        }
    }
}


