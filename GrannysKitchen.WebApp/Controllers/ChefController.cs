using Microsoft.AspNetCore.Mvc;

namespace GrannysKitchen.WebApp.Controllers
{
    public class ChefController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
