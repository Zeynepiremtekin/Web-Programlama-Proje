using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalonController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public SalonController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 1. Salonları Listeleme
        public IActionResult ViewSalons()
        {
            var salons = _dbContext.Salons.ToList();
            return View(salons);
        }

        // 2. Yeni Salon Ekleme - GET
        [HttpGet]
        public IActionResult CreateSalon()
        {
            return View();
        }

        // 3. Yeni Salon Ekleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSalon(Salon salon)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Salons.Add(salon);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewSalons");
            }

            return View(salon);
        }

        // 4. Salon Düzenleme - GET
        [HttpGet]
        public IActionResult EditSalon(int id)
        {
            var salon = _dbContext.Salons.Find(id);
            if (salon == null)
                return NotFound();

            return View(salon);
        }

        // 5. Salon Düzenleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSalon(Salon salon)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Salons.Update(salon);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewSalons");
            }

            return View(salon);
        }

        // 6. Salon Silme
        [HttpPost]
        public IActionResult DeleteSalon(int id)
        {
            var employees = _dbContext.Employees.Where(e => e.SalonId == id).ToList();
            if (employees.Any())
            {
                _dbContext.Employees.RemoveRange(employees);
            }

            var salon = _dbContext.Salons.Find(id);
            if (salon != null)
            {
                _dbContext.Salons.Remove(salon);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("ViewSalons");
        }
    }
}
