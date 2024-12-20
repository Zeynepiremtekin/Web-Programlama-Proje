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
            // Kullanýcýnýn oturum açýp açmadýðýný kontrol ediyoruz
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserFullName = User.Identity.Name; // Kullanýcý adýný ViewBag'e atýyoruz
            }
            else
            {
                ViewBag.UserFullName = null;
            }

            // Veritabanýndan servisleri alýyoruz
            var services = _dbContext.Services.ToList();

            // Eðer servis yoksa boþ bir liste gönderiyoruz
            if (services == null || !services.Any())
            {
                return View(new List<Service>());
            }

            return View(services);
        }

        public IActionResult Privacy()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserFullName = User.Identity.Name;
            }
            else
            {
                ViewBag.UserFullName = null;
            }

            return View();
        }

        public IActionResult AboutUs()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserFullName = User.Identity.Name;
            }
            else
            {
                ViewBag.UserFullName = null;
            }

            return View();
        }

        public IActionResult ContactUs()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserFullName = User.Identity.Name;
            }
            else
            {
                ViewBag.UserFullName = null;
            }

            return View();
        }

        public IActionResult Salons()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserFullName = User.Identity.Name;
            }
            else
            {
                ViewBag.UserFullName = null;
            }

            var salons = _dbContext.Salons.ToList();

            if (salons == null || !salons.Any())
            {
                // Boþ bir liste gönderiyoruz
                return View(new List<Salon>());
            }

            return View(salons);
        }

        public IActionResult Image()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserFullName = User.Identity.Name;
            }
            else
            {
                ViewBag.UserFullName = null;
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

