using AutoMapper;
using GrannysKitchen.API.Authorization;
using GrannysKitchen.API.Services;
using GrannysKitchen.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace GranysKitchen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        public AccountController(
            IUserService userService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [AllowAnonymous]
        [HttpPost("ChefRegistration")]
        public IActionResult ChefRegistration(RegisterRequest model)
        {
            var response = _userService.ChefRegistration(model);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("ChefAuthenticate")]
        public IActionResult ChefAuthenticate(AuthenticateRequest model)
        {
            var response = _userService.ChefAuthenticate(model);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("UserRegistration")]
        public IActionResult UserRegistration(RegisterRequest model)
        {
            var response = _userService.UserRegistration(model);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("UserAuthenticate")]
        public IActionResult UserAuthenticate(AuthenticateRequest model)
        {
            var response = _userService.UserAuthenticate(model);
            return Ok(response);
        }
    }
}
      


