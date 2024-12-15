using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 1. Çalışanları Listeleme
        public IActionResult ViewEmployees()
        {
            var employees = _dbContext.Employees
                .Include(e => e.Salon)
                .ToList();

            return View(employees);
        }

        // 2. Yeni Çalışan Ekleme - GET
        [HttpGet]
        public IActionResult AddEmployee()
        {
            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                })
                .ToList();

            return View();
        }

        // 3. Yeni Çalışan Ekleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Employees.Add(employee);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewEmployees");
            }

            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                })
                .ToList();

            return View(employee);
        }

        // 4. Çalışan Düzenleme - GET
        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var employee = _dbContext.Employees.Find(id);
            if (employee == null)
                return NotFound();

            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                })
                .ToList();

            return View(employee);
        }

        // 5. Çalışan Düzenleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Employees.Update(employee);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewEmployees");
            }

            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                })
                .ToList();

            return View(employee);
        }

        // 6. Çalışan Silme
        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _dbContext.Employees.Find(id);
            if (employee != null)
            {
                _dbContext.Employees.Remove(employee);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("ViewEmployees");
        }
    }
}
