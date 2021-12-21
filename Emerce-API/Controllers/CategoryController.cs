using Emerce_API.Infrastructure;
using Emerce_Model;
using Emerce_Model.Category;
using Emerce_Service.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Emerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoryController( ICategoryService _categoryService, IDistributedCache _redisCache ) : base(_redisCache)
        {
            categoryService = _categoryService;
        }
        //Insert Category
        [HttpPost]
        [LoginFilter]
        [AdminFilter]
        public General<CategoryViewModel> Insert( [FromBody] CategoryCreateModel newCategory )
        {
            newCategory.Iuser = CurrentUser.Id;
            return categoryService.Insert(newCategory);
        }
        //Get Category
        [HttpGet]
        [LoginFilter]
        public General<CategoryViewModel> Get()
        {
            return categoryService.Get();
        }
        //Get Category By Id
        [HttpGet("{id}")]
        [LoginFilter]
        public General<CategoryViewModel> GetById( int id )
        {
            return categoryService.GetById(id);
        }
        //Update Category

        [HttpPut("{id}")]
        [LoginFilter]
        [AdminFilter]
        public General<CategoryViewModel> Update( [FromBody] CategoryUpdateModel updatedCategory, int id )
        {
            updatedCategory.Uuser = CurrentUser.Id;
            return categoryService.Update(updatedCategory, id);
        }

        //Delete User = throws ex if user is not found

        [HttpDelete("{id}")]
        [LoginFilter]
        [AdminFilter]
        public General<CategoryViewModel> Delete( int id )
        {
            return categoryService.Delete(id);
        }
    }
}
