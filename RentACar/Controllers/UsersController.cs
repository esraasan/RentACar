using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Models;
using RentACar.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using RentACar.Repository;

namespace RentACar.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _appDbContext;

        public UsersController(IUsersRepository usersRepository, IWebHostEnvironment webHostEnvironment, AppDbContext appDbContext)
        {
            _usersRepository = usersRepository;
            _webHostEnvironment = webHostEnvironment;
            _appDbContext = appDbContext;
        }


        public IActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Name,Surname,Email,Password,PhoneNumber,Address")] Users users)
        {
            if (ModelState.IsValid)
            {
                users.Password = HashPassword(users.Password);
                _appDbContext.Add(users);
                await _appDbContext.SaveChangesAsync();
                return RedirectToAction("Login", "Users");
            }
            return View(users);
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Index()
        {
            var userMail = HttpContext.User.FindFirst("userMail").Value.ToString();
            List<Users> objUserList = _usersRepository.GetAll().ToList();
            return View(objUserList);
        }

        public IActionResult AddUpdate(int? id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (id == null || id == 0)
            {
                return View();
            }

            // Update
            else
            {
                Users? userDb = _usersRepository.Get(u => u.Id == id);
                if (userDb == null)
                {
                    return NotFound();
                }
                return View(userDb);
            }
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public IActionResult AddUpdate(Users user)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                if (user.Id == 0)
                {
                    _usersRepository.Add(user);
                    TempData["Succeed"] = "User added successfully";
                }
                else
                {
                    Users? userDb = _usersRepository.Get(u => u.Id == user.Id);
                    if (userDb == null)
                    {
                        return NotFound();
                    }

                    userDb.Name = user.Name;
                    userDb.Email = user.Email;
                    userDb.Surname = user.Surname;
                    userDb.Password = user.Password;
                    userDb.PhoneNumber = user.PhoneNumber;
                    user.Address = user.Address;

                    _usersRepository.Update(userDb);
                    TempData["Succeed"] = "User updated successfully";
                }
                _usersRepository.Save();
                return RedirectToAction("Index", "Users");
            }
            return View(user);
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Users userDb = _usersRepository.Get(u => u.Id == id);
            if (userDb == null)
            {
                return NotFound();
            }
            return View(userDb);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Users? user = _usersRepository.Get(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            _usersRepository.Delete(user);
            _usersRepository.Save();
            TempData["Succeed"] = "User removed successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Login(string userMail, string password)
        {
            if (ModelState.IsValid)
            {
                var user = _usersRepository.Get(u => u.Email == userMail);
                if (user != null && VerifyPassword(password, user.Password))
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim("userMail",user.Email),
                        new Claim("userId",user.Id.ToString()),
                        new Claim("userName", user.Name), /*Kullanıcı adını ekliyomm*/
                        new Claim("type",user.UserType)
                    };
                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(principal);
                    return user.UserType == "Admin" ? RedirectToAction("Index") : RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            string hashedInputPassword = HashPassword(inputPassword);
            return hashedInputPassword.Equals(hashedPassword);
        }

       
        public IActionResult Profile()
        {
           return View();
        }
    }
}
