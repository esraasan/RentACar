using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using RentACar.Models;
using RentACar.Data;
using RentACar.Repository;

namespace RentACar.Controllers
{
    public class CarsController : Controller
    {
        //private const object UserRoles;
        private readonly ICarsRepository _carsRepository;  //birden fazla kullanırken eklemeler yapılmalı
        public readonly IWebHostEnvironment _webHostEnvironment;
        public CarsController(ICarsRepository carsRepository, IWebHostEnvironment webHostEnvironment)
        {
            _carsRepository = carsRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Index()
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            List<Cars> objCarList = _carsRepository.GetAll().ToList();
            //List<Cars> objCarList = _carsRepository.GetAll(includeProps).ToList();
            return View(objCarList);
        }

        //Get
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AddUpdate(int? id)
        {
            IEnumerable<SelectListItem> CarsList = _carsRepository.GetAll().Select(k => new SelectListItem
            {
                Text = k.CarName,
                Value = k.Id.ToString()
            }
           );
            ViewBag.CarsList = CarsList;

            if (id == null || id == 0) //ekleme
            {
                return View();
            }
            else //guncelleme
            {
                Cars? carVt = _carsRepository.Get(u => u.Id == id); // filtreleme yapıyor.gönderilen id idye eşit olanı gönder (Expression<Func<T, bool>> filtre)
                if (carVt == null)
                {
                    return NotFound();
                }
                return View(carVt);
            }

        }
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AddUpdate(Cars car, IFormFile file)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors); // herhangi bir hatayı buglamak için yazılır.Genellikle tespit için kullanılır.
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string carPath = Path.Combine(wwwRootPath, @"img");

                if (file != null)
                {
                    using (var fileStream = new FileStream(Path.Combine(carPath, file.FileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    car.ImgUrl = @"\img\" + file.FileName;
                }
                if (car.Id == 0)
                {
                    _carsRepository.Add(car);
                    TempData["basarili"] = "The add process was successfully performed"; // uyarı mesajları için kullanılıyor.
                }
                else
                {
                    _carsRepository.Update(car);
                    TempData["basarili"] = " The update process was successfully performed";

                }

                _carsRepository.Save();   // kayıt etmezsen veritabanına işlenmez.
                return RedirectToAction("Index", "Cars");
            }
            return View();
        }
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete(int? id) //soru işaretini temkinli olmak için koy çünkü int olup olmadığını kontrol etmek zorunda
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Cars? carVt = _carsRepository.Get(u => u.Id == id);
            if (carVt == null)
            {
                return NotFound();
            }
            return View(carVt);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult DeletePost(int? id)
        {
            Cars? car = _carsRepository.Get(u => u.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            _carsRepository.Delete(car);
            _carsRepository.Save();
            TempData["basarili"] = "The delete process was successfully performed";
            return RedirectToAction("Index", "Cars");

        }
    }
}
