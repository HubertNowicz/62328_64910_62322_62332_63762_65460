using Microsoft.AspNetCore.Mvc;

namespace Organizer_przepisów_kulinarnych.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
