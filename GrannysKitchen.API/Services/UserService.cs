using AutoMapper;
using GrannysKitchen.API.Authorization;
using GrannysKitchen.Models.Data;
using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using Org.BouncyCastle.Crypto.Generators;

namespace GrannysKitchen.API.Services
{
    public interface IUserService
    {
        Users GetById(int id);
        ChefUsers GetChefUserById(int id);
        ApiResponseMessage ChefRegistration(RegisterRequest model);
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

        public ApiResponseMessage ChefRegistration(RegisterRequest model)
        {
            ApiResponseMessage response = new ApiResponseMessage();
            // validate
            if (_context.Users.Any(x => x.Username == model.Username))
            {
                response.ErrorMessage = "Username '" + model.Username + "' is already taken";
                response.IsSuccess = false;
                return response;
            }

            // map model to new user object
            var user = _mapper.Map<ChefUsers>(model);

            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // save user
            _context.ChefUsers.Add(user);
            _context.SaveChanges();
            response.SuccessMessage = "Registration successful";
            response.IsSuccess = true;
            return response;
        }
    }
}
