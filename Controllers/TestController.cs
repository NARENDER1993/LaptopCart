using Microsoft.AspNetCore.Mvc;

namespace LaptopCart.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
