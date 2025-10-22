using Microsoft.AspNetCore.Mvc;

namespace Project_1.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
