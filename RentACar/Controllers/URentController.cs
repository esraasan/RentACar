using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RentACar.Models;
using RentACar.Repository;
using System;
using System.Linq;

namespace RentACar.Controllers
{
    public class URentController : Controller
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly ICarsRepository _carsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public URentController(IRentalRepository rentalRepository, ICarsRepository carsRepository, IUsersRepository usersRepository, IWebHostEnvironment webHostEnvironment)
        {
            _rentalRepository = rentalRepository;
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
            var car = _carsRepository.Get(c => c.Id == carId);
            if (car == null)
            {
                return NotFound();
            }

            ViewBag.CarId = carId;
            ViewBag.CarName = car.CarName;
            ViewBag.CarBrandName = car.CarBrandName;

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult RentCar(int carId, DateTime StartDate, DateTime EndDate)
        {
            var userIdClaim = HttpContext.User.FindFirst("userId")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var car = _carsRepository.Get(c => c.Id == carId);
            if (car == null)
            {
                return NotFound();
            }

            var urental = new Rental
            {
                CarId = car.Id,
                CarName = car.CarName,
                CarBrandName = car.CarBrandName,
                UserId = userId,
                StartDate = StartDate,
                EndDate = EndDate,
                CarPrice = car.CarPrice
            };

            // Kiralama fiyatını hesapla
            urental.CalculateRentalPrice(car.CarPrice); 

            _rentalRepository.Add(urental);
            _rentalRepository.Save();


            _rentalRepository.Add(urental);
            _rentalRepository.Save();

            TempData["Success"] = "Car rented successfully!";
            return RedirectToAction("RentList");
        }

        [Authorize(Policy = "UserPolicy")]
        public IActionResult RentList()
        {
            var userIdClaim = HttpContext.User.FindFirst("userId")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var userRentals = _rentalRepository.GetAll().Where(r => r.UserId == userId).ToList();
            return View(userRentals);
        }
        [Authorize(Policy = "UserPolicy")]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new Rental());
            }
            else
            {
                var rent = _rentalRepository.Get(r => r.Id == id);
                if (rent == null)
                {
                    return NotFound();
                }
                return View(rent);

            }

        }


        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult Update(Rental rent)
        {
            if (ModelState.IsValid)
            {
                var existingRental = _rentalRepository.Get(r => r.Id == rent.Id);
                if (existingRental == null)
                {
                    return NotFound();
                }

               //önceki fiyatı hesaplama
                var car = _carsRepository.Get(c => c.Id == existingRental.CarId);
                if (car == null)
                {
                    return NotFound();
                }
                double oldPrice = existingRental.CarPrice;
                existingRental.CalculateRentalPrice(car.CarPrice);

                // tarşhleri güncellemek için eklendi
                existingRental.StartDate = rent.StartDate;
                existingRental.EndDate = rent.EndDate;

                _rentalRepository.Update(existingRental);
                TempData["basarili"] = "The rental transaction was successfully completed.";
                _rentalRepository.Save();

                // güncelleme sonrası yeni fiyatın belirlenmesi için
                if (oldPrice != existingRental.CarPrice)
                {
                    TempData["priceUpdated"] = "Rental price updated.";
                }

                return RedirectToAction("RentList");
            }
            return View(rent);
        }




        [Authorize(Policy = "UserPolicy")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var rent = _rentalRepository.Get(r => r.Id == id, includeProperties: "Car");
            if (rent == null)
            {
                return NotFound();
            }

            return View(rent);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult DeletePost(int id)
        {
            var rent = _rentalRepository.Get(r => r.Id == id);
            if (rent == null)
            {
                return NotFound();
            }

            _rentalRepository.Delete(rent);
            _rentalRepository.Save();
            TempData["basarili"] = "The deletion was successfully completed.";
            return RedirectToAction("RentList");
        }
    }
}
