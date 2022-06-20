using AutoMapper;
using GrannysKitchen.Models.Data;
using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GrannysKitchen.API.Services
{
    public interface ICategoriesService
    {
        List<Categories> GetAllCategories();
        Categories GetCategoryById(int id);
        Task<ApiResponseMessage> CreateCategory(CategoriesRequest categoriesRequest);
        Task<ApiResponseMessage> EditCategory(CategoriesRequest categoriesRequest);
        Task<ApiResponseMessage> DeleteConfirmed(int id);
    }
    public class CategoriesService : ICategoriesService
    {
        private GrannysKitchenDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesService(
            GrannysKitchenDbContext context,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public List<Categories> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public Categories GetCategoryById(int id)
        {
            return _context.Categories.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public async Task<ApiResponseMessage> CreateCategory(CategoriesRequest categoriesRequest)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (categoriesRequest != null)
            {
                Categories category = new Categories
                {
                    CategoryName = categoriesRequest.CategoryName,
                    CategoryImage = categoriesRequest.ExistingImage,
                    CreatedBy = categoriesRequest.CreatedBy,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "Category Saved Successfully...";
                return apiResponseMessage;
            }
            return apiResponseMessage;
        }

        public async Task<ApiResponseMessage> EditCategory(CategoriesRequest categoriesRequest)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (categoriesRequest != null)
            {
                var categoryObject = GetCategoryById(categoriesRequest.Id);
                Categories category = new Categories
                {
                    Id = categoriesRequest.Id,
                    CategoryName = categoriesRequest.CategoryName,
                    CategoryImage = categoriesRequest.ExistingImage,
                    ModifiedBy = categoriesRequest.ModifiedBy,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedBy = categoryObject.CreatedBy,
                    CreatedDate = categoryObject.CreatedDate,
                    IsActive = categoryObject.IsActive
                };

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "Category Updated Successfully...";
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
                var category = GetCategoryById(id);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "Category Saved Successfully...";
            }
            return apiResponseMessage;
        }
    }
}