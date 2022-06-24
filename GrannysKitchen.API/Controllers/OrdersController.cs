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
    public class OrdersController : ControllerBase
    {
        private IOrderService _orderService;
        private IMapper _mapper;

        public OrdersController(
            IOrderService orderService,
            IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost("SaveOrderDetails")]
        public async Task<IActionResult> SaveOrderDetails(OrderSummary orderSummary)
        {
            var response = await _orderService.SaveMyOrderDetails(orderSummary);
            return Ok(response);
        }
    }
}
