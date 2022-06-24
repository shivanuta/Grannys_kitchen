using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.ResponseModels;
using GrannysKitchen.WebApp.Authorization;
using GrannysKitchen.WebApp.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
namespace GrannysKitchen.WebApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly IConfiguration _Configure;
        private static string apiBaseUrl;
        public CartController(ILogger<CartController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _Configure = configuration;
            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }
        //public IActionResult OrderSummary(OrderSummary orderSummary)
        //{
        //    return Ok();
        //}
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItems>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            //if (cart != null)
            //{
            //    ViewBag.total = cart.Sum(item => (item.BooksDetailsResponse.ActualPrice - (item.BooksDetailsResponse.ActualPrice * item.BooksDetailsResponse.DiscountPercentage / 100)) * item.Quantity);
            //}
            return View();
        }
        [Route("AddToCart/{id}")]
        public async Task<IActionResult> AddToCart(int id)
        {
            List<FoodItemsResponse> productModel = await GetAllBooksDetails(string.Empty);
            if (SessionHelper.GetObjectFromJson<List<CartItems>>(HttpContext.Session, "cart") == null)
            {
                List<CartItems> cart = new List<CartItems>();
                cart.Add(new CartItems { FoodItemsResponse = productModel.First(x => x.Id == id), Quantity = 1 });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<CartItems> cart = SessionHelper.GetObjectFromJson<List<CartItems>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new CartItems { FoodItemsResponse = productModel.First(x => x.Id == id), Quantity = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }
        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<CartItems> cart = SessionHelper.GetObjectFromJson<List<CartItems>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> SaveOrder(OrderSummary orderSummary)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                client.DefaultRequestHeaders.Add("Authorization", token);
                orderSummary.CreatedBy = HttpContext.Session.GetInt32("UserId");
                StringContent content = new StringContent(JsonConvert.SerializeObject(orderSummary), Encoding.UTF8, "application/json");
                string endpoint = apiBaseUrl + "Orders/SaveOrderDetails";
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<ApiResponseMessage>(apiResponse);
                    return Ok(responseMessage);
                    //if (responseMessage != null && responseMessage.IsSuccess && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    //{
                    //    ModelState.Clear();
                    //    TempData["Message"] = responseMessage.SuccessMessage;
                    //    return RedirectToAction("Index");
                    //}
                    //else
                    //{
                    //    ModelState.Clear();
                    //    ModelState.AddModelError(string.Empty, responseMessage.ErrorMessage);
                    //    return View("Index", orderSummary);
                    //}
                }
            }
        }
        private int isExist(int id)
        {
            List<CartItems> cart = SessionHelper.GetObjectFromJson<List<CartItems>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].FoodItemsResponse.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
        private async Task<List<FoodItemsResponse>> GetAllBooksDetails(string searchString)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("Token");
                string endpoint = apiBaseUrl + "Shop/GetBooksDetails/" + searchString;
                using (var Response = await client.GetAsync(endpoint))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    var responseMessage = JsonConvert.DeserializeObject<List<FoodItemsResponse>>(apiResponse);
                    if (responseMessage != null && Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return responseMessage;
                    }
                    else
                    {
                        return new List<FoodItemsResponse>();
                    }
                }
            }
        }
    }
}