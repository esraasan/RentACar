using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RentACar.Models;
using RentACar.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using RentACar.Repository;

namespace RentACar.Controllers
{
    public class RentalController : Controller
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly ICarsRepository _carsRepository;
        private readonly IUsersRepository _usersRepository;

        public RentalController(IRentalRepository rentalRepository, ICarsRepository carsRepository, IUsersRepository usersRepository)
        {
            _rentalRepository = rentalRepository;
            _carsRepository = carsRepository;
            _usersRepository = usersRepository;
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Index()
        {
            var rent = _rentalRepository.GetAll(includeProperties: "Cars,Users").ToList();
            var rent2 = _rentalRepository.GetAllRents();
            
            return View(rent2);
        }
        private void SetViewBags()
        {
            ViewBag.Cars = _carsRepository.GetAll().Select(c => new SelectListItem
            {
                Text = c.CarName,
                Value = c.Id.ToString()
            }).ToList();

            ViewBag.Brands = _carsRepository.GetAll().Select(b => new SelectListItem
            {
                Text = b.CarBrandName,
                Value = b.CarBrandName
            }).Distinct().ToList();

            ViewBag.Users = _usersRepository.GetAll().Select(u => new SelectListItem
            {
                Text = $"{u.Name} {u.Surname}",
                Value = u.Id.ToString()
            }).ToList();
        }

        [HttpGet]
        public JsonResult GetCarsByBrand(string brandName)
        {
            var cars = _carsRepository.GetAll().Where(c => c.CarBrandName == brandName).Select(c => new
            {
                Id = c.Id,
                CarName = c.CarName
            }).ToList();

            return Json(cars);
        }
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AddUpdate(int? id)
        {
            SetViewBags();

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
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AddUpdate(Rental rent)
        {
            if (ModelState.IsValid)
            {
                var car = _carsRepository.Get(c => c.Id == rent.CarId);
                if (car != null)
                {
                    rent.CarBrandName = car.CarBrandName;
                }

                if (rent.Id == 0)
                {
                    _rentalRepository.Add(rent);
                    TempData["basarili"] = "The rental transaction was successfully completed.";
                }
                else
                {
                    _rentalRepository.Update(rent);
                    TempData["basarili"] = "The update was successfully completed.";
                }

                _rentalRepository.Save();
                return RedirectToAction("Index");
            }

            SetViewBags();
            return View(rent);
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var rent = _rentalRepository.Get(r => r.Id == id, includeProperties: "Car,User");
            if (rent == null)
            {
                return NotFound();
            }

            return View(rent);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "AdminPolicy")]
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
            return RedirectToAction("Index");
        }


    }
}
