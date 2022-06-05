using AutoMapper;
using GrannysKitchen.API.Authorization;
using GrannysKitchen.Models.Data;
using GrannysKitchen.Models.DBModels;

namespace GrannysKitchen.API.Services
{
    public interface IUserService
    {
        Users GetById(int id);
        ChefUsers GetChefUserById(int id);
    }
    public class UserService : IUserService
    {
        private GrannysKitchenDbContext _context;
        private IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;

        public UserService(
            GrannysKitchenDbContext context,
            IJwtUtils jwtUtils,
            IMapper mapper)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
        }


        public Users GetById(int id)
        {
            return getUser(id);
        }
        public ChefUsers GetChefUserById(int id)
        {
            return getChefUser(id);
        }
        private Users getUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
        private ChefUsers getChefUser(int id)
        {
            var user = _context.ChefUsers.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
    }
}
