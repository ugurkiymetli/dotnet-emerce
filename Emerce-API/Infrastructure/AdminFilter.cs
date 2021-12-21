using Emerce_Model.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Emerce_API.Infrastructure
{
    public class AdminFilter : Attribute, IActionFilter
    {
        public void OnActionExecuted( ActionExecutedContext context )
        {
            return;
        }
        public void OnActionExecuting( ActionExecutingContext context )
        {
            var redisCache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
            var loginUser = new UserViewModel();
            string jsonCacheItem;
            var loginUserFromCache = redisCache.Get("Login");
            if ( loginUserFromCache is not null )
            {
                jsonCacheItem = Encoding.UTF8.GetString(loginUserFromCache);
                loginUser = JsonConvert.DeserializeObject<UserViewModel>(jsonCacheItem);
            }

            if ( !loginUser.IsAdmin )
            {
                context.Result = new BadRequestObjectResult("Please login as admin!");
            }
            return;
        }
    }
}


