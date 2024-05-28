using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RentACar.Models;
using RentACar.Repository;
using System.Security.Claims;

namespace RentACar.Controllers
{
    public class URentController : Controller
    {
        private readonly IURentRepository _urentRepository;
        private readonly ICarsRepository _carsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public URentController(IURentRepository urentRepository, ICarsRepository carsRepository, IUsersRepository usersRepository, IWebHostEnvironment webHostEnvironment)
        {
            _urentRepository = urentRepository;
            _carsRepository = carsRepository;
            _usersRepository = usersRepository;
            _webHostEnvironment = webHostEnvironment;   
        }

        public IActionResult Index()
        {
            var cars = _carsRepository.GetAllCars();
            return View(cars);
        }
        public IActionResult RentCar(int carId)
        {
            ViewBag.CarId = carId;
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
       public IActionResult RentCar(int carId, DateTime StartDate, DateTime EndDate)

        {
            var userIdClaim = HttpContext.User.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var car = _carsRepository.Get(c => c.Id == carId);
            if (car == null)
            {
                return NotFound();
            }

            var urental = new URent
            {
                CarId = car.Id,
                CarBrandName=car.CarBrandName,
                UserId = userId,
                StartDate = StartDate,
                EndDate = EndDate
            };

            _urentRepository.Add(urental);
            _urentRepository.Save();

            TempData["Success"] = "Car rented successfully!";
            return RedirectToAction("Index");
        }
    }
}
