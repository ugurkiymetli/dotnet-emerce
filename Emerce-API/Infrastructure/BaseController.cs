using Emerce_Model.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Emerce_API.Infrastructure
{
    public class BaseController : ControllerBase
    {
        private readonly IDistributedCache redisCache;
        public BaseController( IDistributedCache _redisCache )
        {
            redisCache = _redisCache;
        }
        public UserViewModel CurrentUser
        {
            get
            {
                return GetCurrentUser();
            }
        }
        private UserViewModel GetCurrentUser()
        {
            var loginUser = new UserViewModel();
            string jsonCacheItem;
            var loginUserFromCache = redisCache.Get("Login");
            if ( loginUserFromCache is not null )
            {
                jsonCacheItem = Encoding.UTF8.GetString(loginUserFromCache);
                loginUser = JsonConvert.DeserializeObject<UserViewModel>(jsonCacheItem);
            }
            return loginUser;
        }
    }
}