using Emerce_API.Infrastructure;
using Emerce_Model;
using Emerce_Model.User;
using Emerce_Service.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Emerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IUserService userService;
        private readonly IDistributedCache redisCache;
        public AuthController( IUserService _userService, IDistributedCache _redisCache ) : base(_redisCache)
        {
            userService = _userService;
            redisCache = _redisCache;
        }

        //Login User = takes General object and returns general object with isSuccess : true/false
        [HttpPost]
        [Route("login")]
        //first get from memory cache. than check if entity not empty and user matches with login user.
        //
        public async Task<General<UserViewModel>> Login( [FromBody] UserLoginModel loginUser )
        {
            General<UserViewModel> response = new() { Entity = null };
            //gets loginUser from Base.Controller.CurrentUser 
            General<UserViewModel> _response = new() { Entity = CurrentUser };
            if ( _response.Entity is not null && _response.Entity.Username == loginUser.Username )
            {
                response.Entity = _response.Entity;
                response.IsSuccess = true;
            }
            else
            {
                _response = userService.Login(loginUser);
                if ( _response.IsSuccess )
                {
                    string cacheKey = "Login";
                    //we convert our user to json 
                    string jsonCacheItem = JsonConvert.SerializeObject(_response.Entity);
                    //convert json to bytes
                    var loginUserToCache = Encoding.UTF8.GetBytes(jsonCacheItem);
                    //set memory cache options (expiration date and such)
                    var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(System.DateTime.Now.AddMinutes(10));
                    //set item to redis memory
                    await redisCache.SetAsync(cacheKey, loginUserToCache, options);
                    response.Entity = _response.Entity;
                    response.IsSuccess = true;
                }
                response.ExceptionMessage = _response.ExceptionMessage;
            }

            return response;
        }
        [HttpGet]
        [LoginFilter]
        [Route("logout")]
        public async Task<General<bool>> Logout()
        {
            General<bool> response = new();
            if ( CurrentUser.Id <= 0 )
            {
                response.ExceptionMessage = "You are not logged in!";
                return response;
            }
            await redisCache.RemoveAsync("Login");
            response.IsSuccess = true;
            return response;
        }
    }
}
