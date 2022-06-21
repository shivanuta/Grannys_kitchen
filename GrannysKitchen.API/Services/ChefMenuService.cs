using AutoMapper;
using GrannysKitchen.Models.Data;
using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GrannysKitchen.API.Services
{
    public interface IChefMenuService
    {
        List<ChefMenu> GetChefMenuList(int chefId);
        ChefMenu GetChefMenuById(int id);
        Task<ApiResponseMessage> CreateChefMenu(ChefMenuRequest chefMenuRequest);
        Task<ApiResponseMessage> EditChefMenu(ChefMenuRequest chefMenuRequest);
        Task<ApiResponseMessage> DeleteConfirmed(int id);
    }
    public class ChefMenuService : IChefMenuService
    {
        private GrannysKitchenDbContext _context;
        private readonly IMapper _mapper;

        public ChefMenuService(
            GrannysKitchenDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<ChefMenu> GetChefMenuList(int chefId)
        {
            return _context.ChefMenu.Include(x => x.Categories).Where(x => x.CreatedBy == chefId).ToList();
        }

        public ChefMenu GetChefMenuById(int id)
        {
            return _context.ChefMenu.Include(x => x.Categories).AsNoTracking().FirstOrDefault(x => x.Id == id);
        }


        public async Task<ApiResponseMessage> CreateChefMenu(ChefMenuRequest chefMenuRequest)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (chefMenuRequest != null)
            {
                ChefMenu chefMenu = new ChefMenu
                {
                    ItemName = chefMenuRequest.ItemName,
                    MenuDate = chefMenuRequest.MenuDate,
                    CategoryId = chefMenuRequest.CategoryId,
                    CreatedBy = chefMenuRequest.CreatedBy,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ChefMenu.Add(chefMenu);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "Menu item Saved Successfully...";
                return apiResponseMessage;
            }
            return apiResponseMessage;
        }


        public async Task<ApiResponseMessage> EditChefMenu(ChefMenuRequest chefMenuRequest)
        {
            ApiResponseMessage apiResponseMessage = new ApiResponseMessage
            {
                IsSuccess = false
            };
            if (chefMenuRequest != null)
            {
                var chefMenuObject = GetChefMenuById(chefMenuRequest.Id);
                ChefMenu chefMenu = new ChefMenu
                {
                    Id = chefMenuRequest.Id,
                    ItemName = chefMenuRequest.ItemName,
                    MenuDate = chefMenuRequest.MenuDate,
                    CategoryId = chefMenuRequest.CategoryId,
                    ModifiedBy = chefMenuRequest.ModifiedBy,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedBy = chefMenuObject.CreatedBy,
                    CreatedDate = chefMenuObject.CreatedDate,
                    IsActive = chefMenuObject.IsActive
                };

                _context.ChefMenu.Update(chefMenu);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "Menu Updated Successfully...";
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
                var item = GetChefMenuById(id);
                _context.ChefMenu.Remove(item);
                await _context.SaveChangesAsync();
                apiResponseMessage.IsSuccess = true;
                apiResponseMessage.ResponseMesssage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                apiResponseMessage.SuccessMessage = "ChefMenu Deleted Successfully...";
            }
            return apiResponseMessage;
        }

    }
}
