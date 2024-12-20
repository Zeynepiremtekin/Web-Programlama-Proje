using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            // Check if the user is authenticated
            ViewBag.UserFullName = User.Identity.IsAuthenticated ? User.Identity.Name : null;

            // Fetch all services from the database
            var services = _dbContext.Services.ToList();

            return View(services);
        }

        public IActionResult Privacy()
        {
            ViewBag.UserFullName = User.Identity.IsAuthenticated ? User.Identity.Name : null;
            return View();
        }

        public IActionResult AboutUs()
        {
            ViewBag.UserFullName = User.Identity.IsAuthenticated ? User.Identity.Name : null;
            return View();
        }

        public IActionResult Services()
        {
            ViewBag.UserFullName = User.Identity.IsAuthenticated ? User.Identity.Name : null;

            // Fetch all services for display in cards
            var services = _dbContext.Services.ToList();

            if (services == null || !services.Any())
            {
                return View(new List<Service>());
            }

            return View(services);
        }

        public IActionResult ServiceDetails(int id)
        {
            ViewBag.UserFullName = User.Identity.IsAuthenticated ? User.Identity.Name : null;

            // Fetch the specific service details
            var service = _dbContext.Services.FirstOrDefault(s => s.ServiceId == id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        public IActionResult ContactUs()
        {
            ViewBag.UserFullName = User.Identity.IsAuthenticated ? User.Identity.Name : null;
            return View();
        }

        public IActionResult Salons()
        {
            ViewBag.UserFullName = User.Identity.IsAuthenticated ? User.Identity.Name : null;

            // Fetch all salons
            var salons = _dbContext.Salons.ToList();

            if (salons == null || !salons.Any())
            {
                return View(new List<Salon>());
            }

            return View(salons);
        }

        public IActionResult Image()
        {
            ViewBag.UserFullName = User.Identity.IsAuthenticated ? User.Identity.Name : null;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
