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
        // 1. Çalışanları Listeleme
        public IActionResult ViewEmployees()
        {
            var employees = _dbContext.Employees
                .Include(e => e.Salon)        // Çalışanın salon bilgisi
                .Include(e => e.Services)    // Çalışanın atanmış hizmetleri
                .ToList();

            return View(employees); // Görünüme gönder
        }


        // 2. Yeni Çalışan Ekleme - GET
        [HttpGet]
        public IActionResult AddEmployee()
        {
            // Salon listesini ve hizmet listesini doldurun
            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                }).ToList();

            ViewBag.ServiceList = _dbContext.Services
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = s.Name
                }).ToList();

            return View();
        }



        // 3. Yeni Çalışan Ekleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEmployee(Employee employee, List<int> selectedServices)
        {
            if (ModelState.IsValid)
            {
                // Seçilen hizmetleri çalışana bağla
                employee.Services = _dbContext.Services
                    .Where(s => selectedServices.Contains(s.ServiceId))
                    .ToList();

                _dbContext.Employees.Add(employee);
                _dbContext.SaveChanges();

                return RedirectToAction("ViewEmployees");
            }

            // Hata durumunda salon ve hizmet listesini tekrar doldurun
            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                }).ToList();

            ViewBag.ServiceList = _dbContext.Services
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = s.Name
                }).ToList();

            return View(employee);
        }


        // 4. Çalışan Düzenleme - GET
        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var employee = _dbContext.Employees
                .Include(e => e.Services) // Çalışanın mevcut hizmetlerini dahil et
                .FirstOrDefault(e => e.EmployeeId == id);

            if (employee == null)
                return NotFound();

            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                }).ToList();

            ViewBag.ServiceList = _dbContext.Services
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = s.Name
                }).ToList();

            return View(employee);
        }


        // 5. Çalışan Düzenleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEmployee(Employee employee, List<int> selectedServices)
        {
            var existingEmployee = _dbContext.Employees
                .Include(e => e.Services) // Mevcut hizmetleri dahil et
                .FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);

            if (existingEmployee != null)
            {
                // Temel bilgileri güncelle
                existingEmployee.Name = employee.Name;
                existingEmployee.Surname = employee.Surname;
                existingEmployee.SalonId = employee.SalonId;

                // Hizmetleri güncelle
                existingEmployee.Services.Clear();
                existingEmployee.Services = _dbContext.Services
                    .Where(s => selectedServices.Contains(s.ServiceId))
                    .ToList();

                _dbContext.SaveChanges();
                return RedirectToAction("ViewEmployees");
            }

            ViewBag.SalonList = _dbContext.Salons.ToList();
            ViewBag.ServiceList = _dbContext.Services.ToList();

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
