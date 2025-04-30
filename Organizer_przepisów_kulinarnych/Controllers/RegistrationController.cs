using Microsoft.AspNetCore.Mvc;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;
using Organizer_przepisów_kulinarnych.DAL.Entities;


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
        public async Task<IActionResult> Register(User model)
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
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

    }
}
