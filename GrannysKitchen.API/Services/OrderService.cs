using AutoMapper;
using GrannysKitchen.Models.Data;
using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GrannysKitchen.API.Services
{
    public interface IOrderService
    {
        Task<ApiResponseMessage> SaveMyOrderDetails(OrderSummary orderSummary);

    }
    public class OrderService : IOrderService
    {
        private GrannysKitchenDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderService(
            GrannysKitchenDbContext context,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ApiResponseMessage> SaveMyOrderDetails(OrderSummary orderSummary)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            string orderRefNo = Guid.NewGuid().ToString();
            if (orderSummary != null)
            {

                foreach (var foodItem in orderSummary.FoodItemRequest)
                {
                    Orders order = new Orders
                    {
                        FoodItemId = foodItem.Id,
                        Quantity = foodItem.Quantity,
                        TotalQuantity = orderSummary.TotalQuantity,
                        TotalPrice = orderSummary.TotalPrice,
                        PriceAfterDiscount = orderSummary.PriceAfterDiscount,
                        OrderReferenceNo = orderRefNo,
                        CreatedBy = orderSummary.CreatedBy,
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.Orders.Add(order);
                }
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "Order Saved Successfully...";
                return apiResponseMessage;
            }
            return apiResponseMessage;
        }

    }
}
