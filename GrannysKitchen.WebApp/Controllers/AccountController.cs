
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using GrannysKitchen.WebApp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GrannysKitchen.WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _Configure;
        private static string apiBaseUrl;

        public AccountController(ILogger<AccountController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _Configure = configuration;

            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }

        //Chef Login View
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        //User Login View
        [AllowAnonymous]
        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChefRegistration(RegisterRequest user)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Account/ChefRegistration";

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<ApiResponseMessage>(apiResponse);

                    if (responseMessage != null && responseMessage.IsSuccess && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ModelState.Clear();
                        TempData["Message"] = responseMessage.SuccessMessage;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, responseMessage.ErrorMessage);
                        return View("Registration");
                    }
                }
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChefLoginCheck(AuthenticateRequest user)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Account/ChefAuthenticate";

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var authenticateResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(apiResponse);

                    if (authenticateResponse != null && authenticateResponse.ResponseMesssage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        HttpContext.Session.SetString("Token", authenticateResponse.Token);
                        HttpContext.Session.SetInt32("IsChef", 1);
                        HttpContext.Session.SetString("Username", authenticateResponse.Username);
                        HttpContext.Session.SetString("Name", authenticateResponse.FirstName + " " + authenticateResponse.LastName);
                        HttpContext.Session.SetInt32("UserId", authenticateResponse.Id);
                        TempData["Profile"] = JsonConvert.SerializeObject(apiResponse);
                        return RedirectToAction("Index", "Chef");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                        return View("Index");
                    }
                }
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UserRegistration(RegisterRequest user)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Account/UserRegistration";
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<ApiResponseMessage>(apiResponse);
                    if (responseMessage != null && responseMessage.IsSuccess && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ModelState.Clear();
                        TempData["Message"] = responseMessage.SuccessMessage;
                        return RedirectToAction("UserLogin");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, responseMessage.ErrorMessage);
                        return View("Registration");
                    }
                }
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UserLoginCheck(AuthenticateRequest user)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Account/UserAuthenticate";
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var authenticateResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(apiResponse);
                    if (authenticateResponse != null && authenticateResponse.ResponseMesssage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        HttpContext.Session.SetString("Token", authenticateResponse.Token);
                        HttpContext.Session.SetInt32("IsChef", 0);
                        HttpContext.Session.SetString("Username", authenticateResponse.Username);
                        HttpContext.Session.SetString("Name", authenticateResponse.FirstName + " " + authenticateResponse.LastName);
                        HttpContext.Session.SetInt32("UserId", authenticateResponse.Id);
                        TempData["Profile"] = JsonConvert.SerializeObject(apiResponse);
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                        return View("Index");
                    }
                }
            }
        }
        [AllowAnonymous]
        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            return View(forgotPasswordRequest);
        }
        [AllowAnonymous]
        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            return View(resetPasswordRequest);
        }
        [AllowAnonymous]
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(ResetPasswordRequest resetPasswordRequest)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(resetPasswordRequest), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Account/ResetPassword";
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.IsSuccessStatusCode)
                    {
                        ModelState.Clear();
                        TempData["PasswordResetSuccessMessage"] = "Please check you email for Password reset link";
                        return View("Index");
                    }
                    else
                    {
                        ModelState.Clear();
                        TempData["ErrorMessage"] = "You are not authorized user";
                        return View("ResetPassword", resetPasswordRequest);
                    }
                }
            }
        }
        [AllowAnonymous]
        [HttpPost("SendPasswordRestLink")]
        public async Task<IActionResult> SendPasswordRestLink(ForgotPasswordRequest forgotPasswordRequest)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(forgotPasswordRequest), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Account/ForgotPassword";
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.IsSuccessStatusCode)
                    {
                        ModelState.Clear();
                        TempData["PasswordResetSuccessMessage"] = "Please check you email for Password reset link";
                        return View("ForgotPassword");
                    }
                    else
                    {
                        ModelState.Clear();
                        TempData["ErrorMessage"] = "Please enter valid registered EmailId";
                        return View("ForgotPassword", forgotPasswordRequest);
                    }
                }
            }
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

