using AutoMapper;
using EmailService;
using GrannysKitchen.API.Authorization;
using GrannysKitchen.API.Services;
using GrannysKitchen.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
namespace GrannysKitchen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private IEmailSender _emailSender;
        public AccountController(
            IUserService userService,
            IMapper mapper,
            IEmailSender emailSender)
        {
            _userService = userService;
            _mapper = mapper;
            _emailSender = emailSender;
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
        [HttpGet("SendTestMail")]
        [AllowAnonymous]
        public IActionResult SendTestEmail()
        {
            var rng = new Random();
            var message = new Message(new string[] { "nutakki.srk369@gmail.com" }, "Test email", "This is the content from our email.");
            _emailSender.SendEmail(message);
            return Ok("Email Sent");
        }
        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            _userService.ForgotPassword(forgotPasswordRequest, "https://localhost:7113");
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            _userService.ResetPassword(resetPasswordRequest);
            return Ok();
        }
    }
} 


