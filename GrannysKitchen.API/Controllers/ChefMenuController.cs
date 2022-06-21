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
    public class ChefMenuController : ControllerBase
    {
        private IChefMenuService _chefMenuService;
        private IMapper _mapper;

        public ChefMenuController(
            IChefMenuService chefMenuService,
            IMapper mapper)
        {
            _chefMenuService = chefMenuService;
            _mapper = mapper;
        }

        [HttpGet("GetChefMenuList/{chefId}")]
        public IActionResult GetChefMenuList(int chefId)
        {
            var response = _chefMenuService.GetChefMenuList(chefId);
            return Ok(response);
        }

        [HttpGet("GetChefMenuItemById/{id}")]
        public IActionResult GetChefMenuItemById(int id)
        {
            var response = _chefMenuService.GetChefMenuById(id);
            return Ok(response);
        }

        [HttpPost("SaveChefMenuItem")]
        public async Task<IActionResult> SaveChefMenuItem(ChefMenuRequest chefMenuRequest)
        {
            var response = await _chefMenuService.CreateChefMenu(chefMenuRequest);
            return Ok(response);
        }


        [HttpPost("EditChefMenu")]
        public async Task<IActionResult> EditChefMenu(ChefMenuRequest chefMenuRequest)
        {
            var response = await _chefMenuService.EditChefMenu(chefMenuRequest);
            return Ok(response);
        }

        [HttpPost("DeleteChefMenuItem/{id}")]
        public async Task<IActionResult> DeleteChefMenuItem(int id)
        {
            var response = await _chefMenuService.DeleteConfirmed(id);
            return Ok(response);
        }
    }
}
