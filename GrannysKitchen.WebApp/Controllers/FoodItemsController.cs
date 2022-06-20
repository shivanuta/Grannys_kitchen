using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using GrannysKitchen.WebApp.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GrannysKitchen.WebApp.Controllers
{
    [Authorize]
    public class FoodItemsController : Controller
    {
        private readonly ILogger<FoodItemsController> _logger;
        private readonly IConfiguration _Configure;
        private static string apiBaseUrl;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FoodItemsController(ILogger<FoodItemsController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _Configure = configuration;
            _webHostEnvironment = webHostEnvironment;
            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }

        public async Task<IActionResult> Index()
        {
            var foodItems = await GetAllFoodItems();
            return View(foodItems);
        }

        public async Task<IActionResult> Create()
        {
            var categoriesList = await GetAllCategories();
            ViewBag.categories = categoriesList;
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodItem = await GetFoodItemById(id.HasValue ? id.Value : 0);
            var foodItemViewModel = new FoodItemsRequest()
            {
                Id = foodItem.Id,
                Name = foodItem.Name,
                Description = foodItem.Description,
                ExistingFoodImage = foodItem.FoodImage,
                CategoryId = foodItem.CategoryId
            };

            if (foodItem == null)
            {
                return NotFound();
            }
            var categoriesList = await GetAllCategories();
            ViewBag.categories = categoriesList;
            return View(foodItemViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodItem = await GetFoodItemById(id.HasValue ? id.Value : 0);

            var foodItemViewModel = new FoodItemsRequest()
            {
                Id = foodItem.Id,
                Name = foodItem.Name,
                Description = foodItem.Description,
                ExistingFoodImage = foodItem.FoodImage
            };
            if (foodItem == null)
            {
                return NotFound();
            }

            return View(foodItemViewModel);
        }

        public async Task<IActionResult> DeleteFoodItem(int id)
        {
            var foodItem = await GetFoodItemById(id);
            using (HttpClient client = new HttpClient())
            {
                string endpoint = apiBaseUrl + "FoodItems/DeleteFoodItem/" + id;
                client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                using (var Response = await client.PostAsync(endpoint, null))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<ApiResponseMessage>(apiResponse);

                    if (responseMessage != null && responseMessage.IsSuccess && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (foodItem.FoodImage != null)
                        {
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder, foodItem.FoodImage);
                            System.IO.File.Delete(filePath);
                        }
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

        public async Task<IActionResult> CreateFoodItem(FoodItemsRequest foodItemsRequest)
        {
            using (HttpClient client = new HttpClient())
            {
                foodItemsRequest.ExistingFoodImage = ProcessUploadedFile(foodItemsRequest);

                var multipartContent = new MultipartFormDataContent();

                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(HttpContext.Session.GetInt32("UserId"))), "CreatedBy");
                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(foodItemsRequest.CategoryId)), "CategoryId");
                multipartContent.Add(new StringContent(foodItemsRequest.Name), "Name");
                multipartContent.Add(new StringContent(foodItemsRequest.Description), "Description");
                multipartContent.Add(new StringContent(foodItemsRequest.ExistingFoodImage), "ExistingFoodImage");
                multipartContent.Add(new StreamContent(foodItemsRequest.FoodImage.OpenReadStream()), "FoodImage", foodItemsRequest.FoodImage.FileName);

                string endpoint = apiBaseUrl + "FoodItems/SaveFoodItem";
                client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                using (var Response = await client.PostAsync(endpoint, multipartContent))
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
                        return View("Create", foodItemsRequest);
                    }
                }
            }
        }

        public async Task<IActionResult> EditFoodItem(FoodItemsRequest foodItemsRequest)
        {
            var foodItem = await GetFoodItemById(foodItemsRequest.Id);
            using (HttpClient client = new HttpClient())
            {
                if (foodItem.FoodImage != null)
                {
                    if (foodItemsRequest.ExistingFoodImage != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder, foodItemsRequest.ExistingFoodImage);
                        System.IO.File.Delete(filePath);
                    }
                    foodItemsRequest.ExistingFoodImage = ProcessUploadedFile(foodItemsRequest);
                }
                var multipartContent = new MultipartFormDataContent();

                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(foodItemsRequest.Id)), "Id");
                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(foodItemsRequest.CategoryId)), "CategoryId");
                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(HttpContext.Session.GetInt32("UserId"))), "ModifiedBy");
                multipartContent.Add(new StringContent(foodItemsRequest.Name), "Name");
                multipartContent.Add(new StringContent(foodItemsRequest.Description), "Description");
                multipartContent.Add(new StringContent(foodItemsRequest.ExistingFoodImage), "ExistingFoodImage");
                multipartContent.Add(new StreamContent(foodItemsRequest.FoodImage.OpenReadStream()), "FoodImage", foodItemsRequest.FoodImage.FileName);

                string endpoint = apiBaseUrl + "FoodItems/EditFoodItem";
                client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                using (var Response = await client.PostAsync(endpoint, multipartContent))
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
                        return View("Edit", foodItemsRequest);
                    }
                }
            }
        }

        #region Private Methods
        private async Task<List<FoodItems>> GetAllFoodItems()
        {
            using (HttpClient client = new HttpClient())
            {
                var chefId = HttpContext.Session.GetInt32("UserId");
                var token = HttpContext.Session.GetString("Token");
                string endpoint = apiBaseUrl + "FoodItems/GetAllFoodItemsOfCurrentChef/" + chefId;
                client.DefaultRequestHeaders.Add("Authorization", token);
                using (var Response = await client.GetAsync(endpoint))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<List<FoodItems>>(apiResponse);

                    if (responseMessage != null && responseMessage.Count() != 0 && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return responseMessage;
                    }
                    else
                    {
                        return new List<FoodItems>();
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

        private string ProcessUploadedFile(FoodItemsRequest model)
        {
            string uniqueFileName = null;

            if (model.FoodImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FoodFileUploadFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FoodImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.FoodImage.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        private async Task<FoodItems> GetFoodItemById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                string endpoint = apiBaseUrl + "FoodItems/GetFoodItemById/" + id;
                client.DefaultRequestHeaders.Add("Authorization", token);
                using (var Response = await client.GetAsync(endpoint))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<FoodItems>(apiResponse);

                    if (responseMessage != null && responseMessage != null && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return responseMessage;
                    }
                    else
                    {
                        return new FoodItems();
                    }
                }
            }
        }
        #endregion

    }
}
