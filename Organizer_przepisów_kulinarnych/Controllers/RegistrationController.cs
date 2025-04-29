using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.BLL.DbContexts;
using Organizer_przepisów_kulinarnych.BLL.Entities.Enums;
using Organizer_przepisów_kulinarnych.BLL.Entities;


namespace Organizer_przepisów_kulinarnych.Controllers
{
    public class RegistrationController : Controller
    {
     

        private readonly ApplicationDbContext _context;

        public RegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Organizer_przepisów_kulinarnych.BLL.Entities.User model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = model.Username,
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    Email = model.Email,
                    PasswordHash = model.PasswordHash, // Uwaga: na produkcji trzeba haszować!
                    UserRole = UserRole.User

                    //UserRole = UserRole.Admin
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

    }
}
