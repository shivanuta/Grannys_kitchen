using AutoMapper;
using GrannysKitchen.API.Authorization;
using GrannysKitchen.API.Services;
using GrannysKitchen.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrannysKitchen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private ICategoriesService _categoriesService;
        private IMapper _mapper;

        public CategoriesController(
            ICategoriesService categoriesService,
            IMapper mapper)
        {
            _categoriesService = categoriesService;
            _mapper = mapper;
        }

        [HttpGet("GetAllCategories")]
        public IActionResult GetAllCategories()
        {
            var response = _categoriesService.GetAllCategories();
            return Ok(response);
        }

        [HttpGet("GetCategoryById/{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var response = _categoriesService.GetCategoryById(id);
            return Ok(response);
        }

        [HttpPost("SaveCategory")]
        public async Task<IActionResult> SaveCategory([FromForm] CategoriesRequest categoriesRequest)
        {
            var response = await _categoriesService.CreateCategory(categoriesRequest);
            return Ok(response);
        }

        [HttpPost("EditCategory")]
        public async Task<IActionResult> EditCategory([FromForm] CategoriesRequest categoriesRequest)
        {
            var response = await _categoriesService.EditCategory(categoriesRequest);
            return Ok(response);
        }

        [HttpPost("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoriesService.DeleteConfirmed(id);
            return Ok(response);
        }
    }
}
