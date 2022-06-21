using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using GrannysKitchen.WebApp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GrannysKitchen.WebApp.Controllers
{
    [Authorize]
    public class ChefMenuController : Controller
    {
        private readonly ILogger<ChefMenuController> _logger;
        private readonly IConfiguration _Configure;
        private static string apiBaseUrl;

        public ChefMenuController(ILogger<ChefMenuController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _Configure = configuration;
            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }

        public async Task<IActionResult> Index()
        {
            var chefMenuItems = await GetChefMenuItemsList();
            return View(chefMenuItems);
        }

        public async Task<IActionResult> Create()
        {
            var categoriesList = await GetAllCategories();
            ViewBag.categories = categoriesList;
            return View();
        }

        public async Task<IActionResult> SaveChefMenuItem(ChefMenuRequest chefMenuRequest)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Add("Authorization", token);
                chefMenuRequest.CreatedBy = HttpContext.Session.GetInt32("UserId");
                StringContent content = new StringContent(JsonConvert.SerializeObject(chefMenuRequest), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "ChefMenu/SaveChefMenuItem";

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
                        return View("Create", chefMenuRequest);
                    }
                }
            }
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chefMenuItem = await GetChefMenuById(id.HasValue ? id.Value : 0);
            var chefMenuViewModel = new ChefMenuRequest()
            {
                Id = chefMenuItem.Id,
                ItemName = chefMenuItem.ItemName,
                MenuDate = chefMenuItem.MenuDate,
                Categories = chefMenuItem.Categories,
                CategoryId = chefMenuItem.CategoryId
            };

            if (chefMenuItem == null)
            {
                return NotFound();
            }
            var categoriesList = await GetAllCategories();
            ViewBag.categories = categoriesList;
            return View(chefMenuViewModel);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chefMenuItem = await GetChefMenuById(id.HasValue ? id.Value : 0);
            var chefMenuViewModel = new ChefMenuRequest()
            {
                Id = chefMenuItem.Id,
                ItemName = chefMenuItem.ItemName,
                MenuDate = chefMenuItem.MenuDate,
                Categories = chefMenuItem.Categories,
                CategoryId = chefMenuItem.CategoryId
            };
            if (chefMenuItem == null)
            {
                return NotFound();
            }

            return View(chefMenuViewModel);
        }


        public async Task<IActionResult> DeleteChefMenuItem(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                string endpoint = apiBaseUrl + "ChefMenu/DeleteChefMenuItem/" + id;
                client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                using (var Response = await client.PostAsync(endpoint, null))
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
                        ModelState.AddModelError(string.Empty, "Delete Failed...");
                        return View("Index");
                    }
                }
            }
        }

        public async Task<IActionResult> EditChefMenuItem(ChefMenuRequest chefMenuRequest)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Add("Authorization", token);
                chefMenuRequest.ModifiedBy = HttpContext.Session.GetInt32("UserId");
                StringContent content = new StringContent(JsonConvert.SerializeObject(chefMenuRequest), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "ChefMenu/EditChefMenu";

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
                        return View("Edit", chefMenuRequest);
                    }
                }
            }
        }


        #region Private Methods
        private async Task<List<ChefMenu>> GetChefMenuItemsList()
        {
            using (HttpClient client = new HttpClient())
            {
                var chefId = HttpContext.Session.GetInt32("UserId");
                var token = HttpContext.Session.GetString("Token");
                string endpoint = apiBaseUrl + "ChefMenu/GetChefMenuList/" + chefId;
                client.DefaultRequestHeaders.Add("Authorization", token);
                using (var Response = await client.GetAsync(endpoint))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<List<ChefMenu>>(apiResponse);

                    if (responseMessage != null && responseMessage.Count() != 0 && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return responseMessage;
                    }
                    else
                    {
                        return new List<ChefMenu>();
                    }
                }
            }
        }


        private async Task<List<Categories>> GetAllCategories()
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                string endpoint = apiBaseUrl + "Categories/GetAllCategories";
                client.DefaultRequestHeaders.Add("Authorization", token);
                using (var Response = await client.GetAsync(endpoint))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<List<Categories>>(apiResponse);

                    if (responseMessage != null && responseMessage.Count() != 0 && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return responseMessage;
                    }
                    else
                    {
                        return new List<Categories>();
                    }
                }
            }
        }


        private async Task<ChefMenu> GetChefMenuById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                string endpoint = apiBaseUrl + "ChefMenu/GetChefMenuItemById/" + id;
                client.DefaultRequestHeaders.Add("Authorization", token);
                using (var Response = await client.GetAsync(endpoint))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<ChefMenu>(apiResponse);

                    if (responseMessage != null && responseMessage != null && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return responseMessage;
                    }
                    else
                    {
                        return new ChefMenu();
                    }
                }
            }
        }


        #endregion
    }
}
