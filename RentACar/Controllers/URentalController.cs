using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RentACar.Models;
using RentACar.Repository;

namespace RentACar.Controllers
{
    public class URentalController : Controller
    {
        private readonly IURentalRepository _urentalRepository;
        private readonly ICarsRepository _carsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public URentalController(IURentalRepository urentalRepository, ICarsRepository carsRepository, IUsersRepository usersRepository, IWebHostEnvironment webHostEnvironment)
        {
            _urentalRepository = urentalRepository;
            _carsRepository = carsRepository;
            _usersRepository = usersRepository;
            _webHostEnvironment = webHostEnvironment;   
        }
        [Authorize(Policy = "UserPolicy")]
        public IActionResult Rental()
        {
            return View();
        }
    }
}
