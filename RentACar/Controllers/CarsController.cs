using Microsoft.AspNetCore.Mvc;

namespace RentACar.Controllers
{
    public class CarsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
