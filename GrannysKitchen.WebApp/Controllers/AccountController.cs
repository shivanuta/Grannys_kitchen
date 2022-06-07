using GrannysKitchen.API.Authorization;
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
                        return RedirectToAction("ChefRegistration");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, responseMessage.ErrorMessage);
                        return View("ChefRegistration");
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
                    var authenticateResponse = JsonConvert.DeserializeObject<ChefAuthenticateResponse>(apiResponse);

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

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

