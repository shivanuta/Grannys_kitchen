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
    public class FoodItemsController : ControllerBase
    {
        private IFoodItemsService _foodItemsService;
        private IMapper _mapper;

        public FoodItemsController(
            IFoodItemsService foodItemsService,
            IMapper mapper)
        {
            _foodItemsService = foodItemsService;
            _mapper = mapper;
        }

        [HttpGet("GetAllFoodItemsOfCurrentChef/{chefId}")]
        public IActionResult GetAllFoodItemsOfCurrentChef(int chefId)
        {
            var response = _foodItemsService.GetAllFoodItemsOfCurrentChef(chefId);
            return Ok(response);
        }

        [HttpGet("GetFoodItemById/{id}")]
        public IActionResult GetFoodItemById(int id)
        {
            var response = _foodItemsService.GetFoodItemById(id);
            return Ok(response);
        }

        [HttpPost("SaveFoodItem")]
        public async Task<IActionResult> SaveFoodItem([FromForm] FoodItemsRequest foodItemsRequest)
        {
            var response = await _foodItemsService.CreateFoodItem(foodItemsRequest);
            return Ok(response);
        }

        [HttpPost("EditFoodItem")]
        public async Task<IActionResult> EditFoodItem([FromForm] FoodItemsRequest foodItemsRequest)
        {
            var response = await _foodItemsService.EditFoodItem(foodItemsRequest);
            return Ok(response);
        }

        [HttpPost("DeleteFoodItem/{id}")]
        public async Task<IActionResult> DeleteFoodItem(int id)
        {
            var response = await _foodItemsService.DeleteConfirmed(id);
            return Ok(response);
        }
    }
}
