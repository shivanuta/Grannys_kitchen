using AutoMapper;
using GrannysKitchen.Models.Data;
using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GrannysKitchen.API.Services
{
    public interface IFoodItemsService
    {
        List<FoodItems> GetAllFoodItemsOfCurrentChef(int chefId);
        FoodItems GetFoodItemById(int id);
        Task<ApiResponseMessage> CreateFoodItem(FoodItemsRequest foodItemsRequest);
        Task<ApiResponseMessage> EditFoodItem(FoodItemsRequest foodItemsRequest);
        Task<ApiResponseMessage> DeleteConfirmed(int id);
        List<FoodItems> GetAllFoodItemsByCategory(int categoryId);
    }
    public class FoodItemsService : IFoodItemsService
    {
        private GrannysKitchenDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FoodItemsService(
            GrannysKitchenDbContext context,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public List<FoodItems> GetAllFoodItemsOfCurrentChef(int chefId)
        {
            return _context.FoodItems.Where(x => x.CreatedBy == chefId).ToList();
        }

        public FoodItems GetFoodItemById(int id)
        {
            return _context.FoodItems.Include(x => x.Categories).AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public async Task<ApiResponseMessage> CreateFoodItem(FoodItemsRequest foodItemsRequest)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (foodItemsRequest != null)
            {
                FoodItems foodItem = new FoodItems
                {
                    Name = foodItemsRequest.Name,
                    Description = foodItemsRequest.Description,
                    CategoryId = foodItemsRequest.CategoryId,
                    FoodImage = foodItemsRequest.ExistingFoodImage,
                    CreatedBy = foodItemsRequest.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    ActualPrice = foodItemsRequest.ActualPrice,
                    DiscountPercentage = foodItemsRequest.DiscountPercentage,
                    TotalStock = foodItemsRequest.TotalStock,
                    DeliveryCharges = foodItemsRequest.DeliveryCharges,
                    AvailableStock = foodItemsRequest.TotalStock
                };

                _context.FoodItems.Add(foodItem);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "FoodItem Saved Successfully...";
                return apiResponseMessage;
            }
            return apiResponseMessage;
        }


        public async Task<ApiResponseMessage> EditFoodItem(FoodItemsRequest foodItemsRequest)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (foodItemsRequest != null)
            {
                var foodItemObject = GetFoodItemById(foodItemsRequest.Id);
                FoodItems foodItem = new FoodItems
                {
                    Id = foodItemsRequest.Id,
                    Name = foodItemsRequest.Name,
                    FoodImage = foodItemsRequest.ExistingFoodImage,
                    Description = foodItemsRequest.Description,
                    CategoryId = foodItemsRequest.CategoryId,
                    ModifiedBy = foodItemsRequest.ModifiedBy,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedBy = foodItemObject.CreatedBy,
                    CreatedDate = foodItemObject.CreatedDate,
                    IsActive = foodItemObject.IsActive,
                    ActualPrice = foodItemsRequest.ActualPrice,
                    DiscountPercentage = foodItemsRequest.DiscountPercentage,
                    TotalStock = foodItemsRequest.TotalStock,
                    DeliveryCharges = foodItemsRequest.DeliveryCharges,
                    AvailableStock = foodItemsRequest.TotalStock
                };

                _context.FoodItems.Update(foodItem);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "FoodItem Updated Successfully...";
                return apiResponseMessage;
            }
            return apiResponseMessage;
        }

        public async Task<ApiResponseMessage> DeleteConfirmed(int id)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (id != 0)
            {
                var item = GetFoodItemById(id);
                _context.FoodItems.Remove(item);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "FoodItem Deleted Successfully...";
            }
            return apiResponseMessage;
        }
        public List<FoodItems> GetAllFoodItemsByCategory(int categoryId)
        {
            return _context.FoodItems.Where(x => x.CategoryId == categoryId).ToList();
        }


    }
}
