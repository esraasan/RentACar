using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RentACar.Models;
using RentACar.Repository;
using System;
using System.Linq;
using System.Runtime.ConstrainedExecution;

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

        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var userIdClaim = HttpContext.User.FindFirst("userId")?.Value;
            var now = DateTime.Now;
            var userRentals = _rentalRepository.GetAll().ToList();

            var allCars = _carsRepository.GetAllCars().ToList();
            var startdate = TempData["StartDate"];
            var enddate = TempData["EndDate"];
            if (startDate.HasValue)
            {
                ViewBag.StartDate = startDate;
            }
            if (endDate.HasValue)
            {
                ViewBag.EndDate = endDate;
            }



            foreach (var rental in userRentals)
            {
                if (rental.EndDate <= now)
                {
                    var car = _carsRepository.Get(c => c.Id == rental.CarId);
                    if (car != null && !car.IsActive)
                    {
                        car.IsActive = true;
                        _carsRepository.Update(car);
                    }
                }
                else
                {
                    var car = _carsRepository.Get(c => c.Id == rental.CarId);
                    car.IsActive = false;
                    _carsRepository.Update(car);
                }
                if (rental.StartDate >= now)
                {
                    var rentalCar = _carsRepository.Get(l => l.Id == rental.CarId);
                    if (!rentalCar.IsActive)
                    {
                        rentalCar.IsActive = true;
                        _carsRepository.Update(rentalCar);
                    }
                }
                else
                {
                    var rentalCar = _carsRepository.Get(l => l.Id == rental.CarId);
                    
                    rentalCar.IsActive = false;
                    _carsRepository.Update(rentalCar);
                }
                _carsRepository.Save();
            }
            
            if (startDate.HasValue && endDate.HasValue)
            {
               
                var unavailableCar = _rentalRepository.GetAll()
                    .Where(r => r.StartDate < endDate && r.EndDate > startDate)
                    .Select(r => r.CarId)
                    .Distinct()
                    .ToList();

                allCars = allCars.Where(car => !unavailableCar.Contains(car.Id)).ToList();
                TempData["StartDate"] = startDate.Value;
                TempData["EndDate"] = endDate.Value;
                return View(allCars);
            }
            var cars = _carsRepository.GetAllCars().Where(c => c.IsActive).ToList();
            return View(cars);
        }



       

        [Authorize(Policy = "UserPolicy")]
        public IActionResult RentList()
        {
            var userIdClaim = HttpContext.User.FindFirst("userId")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var now = DateTime.UtcNow;
            var userRentals = _rentalRepository.GetAll().Where(r => r.UserId == userId).ToList();

            foreach (var rental in userRentals)
            {
                if (rental.EndDate <= now)
                {
                    var car = _carsRepository.Get(c => c.Id == rental.CarId);
                    if (car != null && !car.IsActive)
                    {
                        car.IsActive = true;
                        _carsRepository.Update(car);
                    }
                }
            }

            _carsRepository.Save();
            return View(userRentals);
        }
        public IActionResult RentCar(int carId, string startDate, string endDate)
        {
            var car = _carsRepository.Get(c => c.Id == carId);
            if (car == null)
            {
                return NotFound();
            }

            ViewBag.CarId = carId;
            ViewBag.CarName = car.CarName;
            ViewBag.CarBrandName = car.CarBrandName;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult RentCar(int carId, string sdate, string edate, int deneme)
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

            // girilen tarihler arasında kiralama varsa uyarı vermesi için 
            //var existingRentals = _rentalRepository.GetAll()
            //    .Where(r => r.CarId == carId &&
            //           ((r.StartDate < EndDate && r.EndDate > StartDate) ||
            //            (r.StartDate == StartDate && r.EndDate == EndDate)))
            //    .ToList();

            //if (existingRentals.Any())
            //{
            //    TempData["Error"] = "The car is already rented for the selected dates.";
            //    ViewBag.CarId = carId;
            //    ViewBag.CarName = car.CarName;
            //    ViewBag.CarBrandName = car.CarBrandName;
            //    return View();
            //}

            var urental = new Rental
            {
                CarId = car.Id,
                CarName = car.CarName,
                CarBrandName = car.CarBrandName,
                UserId = userId,
                StartDate = DateTime.Parse(sdate),
                EndDate = DateTime.Parse(edate),
                CarPrice = car.CarPrice
            };

            urental.CalculateRentalPrice(car.CarPrice);

            _rentalRepository.Add(urental);
            _rentalRepository.Save();

            var now = DateTime.UtcNow;
            var userRentals = _rentalRepository.GetAll().Where(r => r.UserId == userId).ToList();

            foreach (var rental in userRentals)
            {
                if (rental.StartDate >= now)
                {
                    var rentalCar = _carsRepository.Get(l => l.Id == rental.CarId);
                    if (rentalCar.IsActive)
                    {
                        rentalCar.IsActive = false;
                        _carsRepository.Update(rentalCar);
                    }
                }
            }
            _carsRepository.Save();

            TempData["Success"] = "Car rented successfully!";
            return RedirectToAction("RentList");
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
        public IActionResult DeletePost(int id,int carId)
        {
            var car = _carsRepository.Get(c => c.Id == carId);
            var rent = _rentalRepository.Get(r => r.Id == id);
            if (rent == null)
            {
                return NotFound();
            }

            _rentalRepository.Delete(rent);
            _rentalRepository.Save();

            car.IsActive = true;
            _carsRepository.Update(car);
            _carsRepository.Save();
            TempData["basarili"] = "The deletion was successfully completed.";
            return RedirectToAction("RentList");
        }
    }
}
