using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.BLL.DbContexts;

namespace Organizer_przepisów_kulinarnych.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Organizer_przepisów_kulinarnych.BLL.Entities.User model)
        {
           

            //if (ModelState.IsValid)
            //{
                var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.PasswordHash == model.PasswordHash);


                if (user != null)
                {
                    Console.WriteLine("Użytkownik znaleziony: " + user.Username);

                    switch (user.UserRole)
                    {
                        case BLL.Entities.Enums.UserRole.Admin:
                            HttpContext.Session.SetInt32("Role", 0);
                            HttpContext.Session.SetString("Username", user.Username);
                            return RedirectToAction("Index", "Admin");

                        case BLL.Entities.Enums.UserRole.User:
                            HttpContext.Session.SetInt32("Role", 1);
                            HttpContext.Session.SetString("Username", user.Username);
                            return RedirectToAction("Index", "User");

                        case BLL.Entities.Enums.UserRole.Moderator:
                            HttpContext.Session.SetInt32("Role", 2);
                            HttpContext.Session.SetString("Username", user.Username);
                            return RedirectToAction("Index", "Moderator");
                        default:
                            Console.WriteLine("Nie rozpoznano roli: " + (int)user.UserRole);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Nie znaleziono użytkownika!");
                }

            //        ModelState.AddModelError(string.Empty, "Nieprawidłowy login lub hasło.");

            //}

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}
