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
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly IConfiguration _Configure;
        private static string apiBaseUrl;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(ILogger<CategoriesController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _Configure = configuration;
            _webHostEnvironment = webHostEnvironment;
            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }

        public async Task<IActionResult> Index()
        {
            var categories = await GetAllCategories();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await GetCategoryById(id.HasValue ? id.Value : 0);
            var categoryViewModel = new CategoriesRequest()
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                ExistingImage = category.CategoryImage
            };

            if (category == null)
            {
                return NotFound();
            }
            return View(categoryViewModel);
        }

        public async Task<IActionResult> CreateCategory(CategoriesRequest categoriesRequest)
        {
            using (HttpClient client = new HttpClient())
            {
                categoriesRequest.ExistingImage = ProcessUploadedFile(categoriesRequest);

                var multipartContent = new MultipartFormDataContent();

                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(HttpContext.Session.GetInt32("UserId"))), "CreatedBy");

                multipartContent.Add(new StringContent(categoriesRequest.CategoryName), "CategoryName");
                multipartContent.Add(new StringContent(categoriesRequest.ExistingImage), "ExistingImage");
                multipartContent.Add(new StreamContent(categoriesRequest.CategoryImage.OpenReadStream()), "CategoryImage", categoriesRequest.CategoryImage.FileName);

                string endpoint = apiBaseUrl + "Categories/SaveCategory";
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
                        return View("Create", categoriesRequest);
                    }
                }
            }
        }

        public async Task<IActionResult> EditCategory(CategoriesRequest categoriesRequest)
        {
            var category = await GetCategoryById(categoriesRequest.Id);
            using (HttpClient client = new HttpClient())
            {
                if (category.CategoryImage != null)
                {
                    if (categoriesRequest.ExistingImage != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder, categoriesRequest.ExistingImage);
                        System.IO.File.Delete(filePath);
                    }
                    categoriesRequest.ExistingImage = ProcessUploadedFile(categoriesRequest);
                }
                var multipartContent = new MultipartFormDataContent();

                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(categoriesRequest.Id)), "Id");
                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(HttpContext.Session.GetInt32("UserId"))), "ModifiedBy");
                multipartContent.Add(new StringContent(categoriesRequest.CategoryName), "CategoryName");
                multipartContent.Add(new StringContent(categoriesRequest.ExistingImage), "ExistingImage");
                multipartContent.Add(new StreamContent(categoriesRequest.CategoryImage.OpenReadStream()), "CategoryImage", categoriesRequest.CategoryImage.FileName);

                string endpoint = apiBaseUrl + "Categories/EditCategory";
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
                        return View("Edit", categoriesRequest);
                    }
                }
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await GetCategoryById(id.HasValue ? id.Value : 0);

            var categoryViewModel = new CategoriesRequest()
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                ExistingImage = category.CategoryImage
            };
            if (category == null)
            {
                return NotFound();
            }

            return View(categoryViewModel);
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await GetCategoryById(id);
            using (HttpClient client = new HttpClient())
            {
                string endpoint = apiBaseUrl + "Categories/DeleteCategory/" + id;
                client.DefaultRequestHeaders.Add("Authorization", HttpContext.Session.GetString("Token"));
                using (var Response = await client.PostAsync(endpoint, null))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<ApiResponseMessage>(apiResponse);

                    if (responseMessage != null && responseMessage.IsSuccess && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (category.CategoryImage != null)
                        {
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder, category.CategoryImage);
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
        private async Task<Categories> GetCategoryById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                string endpoint = apiBaseUrl + "Categories/GetCategoryById/" + id;
                client.DefaultRequestHeaders.Add("Authorization", token);
                using (var Response = await client.GetAsync(endpoint))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<Categories>(apiResponse);

                    if (responseMessage != null && responseMessage != null && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return responseMessage;
                    }
                    else
                    {
                        return new Categories();
                    }
                }
            }
        }
        private string ProcessUploadedFile(CategoriesRequest model)
        {
            string uniqueFileName = null;

            if (model.CategoryImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CategoryImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CategoryImage.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
