using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ServiceController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult ViewServices()
        {
            var services = _dbContext.Services.ToList();
            return View(services);
        }

        [HttpGet]
        public IActionResult CreateService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateService(Service service)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Services.Add(service);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewServices");
            }
            return View(service);
        }

        [HttpGet]
        public IActionResult EditService(int id)
        {
            var service = _dbContext.Services.Find(id);
            if (service == null) return NotFound();

            return View(service);
        }

        [HttpPost]
        public IActionResult EditService(Service service)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Services.Update(service);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewServices");
            }
            return View(service);
        }

        [HttpPost]
        public IActionResult DeleteService(int id)
        {
            var service = _dbContext.Services.Find(id);
            if (service != null)
            {
                _dbContext.Services.Remove(service);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("ViewServices");
        }
    }
}
