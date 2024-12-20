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
                .Include(e => e.Services)
                .ToList();

            var employeeEfficiencies = employees.Select(employee =>
            {
                var totalWorkingMinutes = CalculateTotalWorkingMinutes(employee.WorkingHours);
                var appointmentsDuration = _dbContext.Appointments
                    .Where(a => a.EmployeeId == employee.EmployeeId && a.IsConfirmed)
                    .Sum(a => a.Service.Duration);

                var efficiency = totalWorkingMinutes > 0 ? (appointmentsDuration / (double)totalWorkingMinutes) * 100 : 0;

                return new
                {
                    EmployeeId = employee.EmployeeId,
                    Efficiency = efficiency
                };
            }).ToList();

            ViewBag.EmployeeEfficiencies = employeeEfficiencies;

            return View(employees);
        }

        private int CalculateTotalWorkingMinutes(string workingHours)
        {
            var hours = workingHours.Split('-');
            if (hours.Length == 2)
            {
                var start = TimeSpan.Parse(hours[0].Trim());
                var end = TimeSpan.Parse(hours[1].Trim());
                return (int)(end - start).TotalMinutes;
            }
            return 0;
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEmployee(Employee employee, List<int> selectedServices)
        {
            if (ModelState.IsValid)
            {
                employee.Services = _dbContext.Services
                    .Where(s => selectedServices.Contains(s.ServiceId))
                    .ToList();

                _dbContext.Employees.Add(employee);
                _dbContext.SaveChanges();

                return RedirectToAction("ViewEmployees");
            }

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

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var employee = _dbContext.Employees
                .Include(e => e.Services)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEmployee(Employee employee, List<int> selectedServices)
        {
            var existingEmployee = _dbContext.Employees
                .Include(e => e.Services)
                .FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);

            if (existingEmployee != null)
            {
                existingEmployee.Name = employee.Name;
                existingEmployee.Surname = employee.Surname;
                existingEmployee.SalonId = employee.SalonId;
                existingEmployee.WorkingHours = employee.WorkingHours; // Çalışma saatlerini güncelle

                existingEmployee.Services.Clear();
                existingEmployee.Services = _dbContext.Services
                    .Where(s => selectedServices.Contains(s.ServiceId))
                    .ToList();

                _dbContext.SaveChanges();
                return RedirectToAction("ViewEmployees");
            }

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
