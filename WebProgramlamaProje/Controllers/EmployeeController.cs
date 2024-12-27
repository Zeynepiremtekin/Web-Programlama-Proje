using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Belirli bir salonun çalışanlarını listele
        public IActionResult GetEmployeesBySalon(int salonId)
        {
            var employees = _dbContext.Employees
                                      .Include(e => e.Services)  // Çalışanla ilişkili hizmetleri dahil et
                                      .Where(e => e.SalonId == salonId)
                                      .ToList();

            if (employees == null || !employees.Any())
            {
                return NotFound("No employees found for this salon.");
            }

            return View(employees);  // Çalışanlar ve hizmetlerle birlikte view'a gönder
        }

        // 1. Çalışanları Listeleme
        [Authorize(Roles = "Admin")]
        public IActionResult ViewEmployees()
        {
            var employees = _dbContext.Employees
                .Include(e => e.Salon)
                .Include(e => e.Services)
                .ToList();

            var employeeEfficiencies = employees.Select(employee =>
            {
                var totalWorkingMinutes = CalculateTotalWorkingMinutes(employee.WorkingHours);

                // Yalnızca "Approved" durumundaki randevuların sürelerini topluyoruz.
                var appointmentsDuration = _dbContext.Appointments
                    .Where(a => a.EmployeeId == employee.EmployeeId && a.Status == "Approved")
                    .Sum(a => a.Service.Duration);

                // Verimlilik hesabı
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

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult AddEmployee(Employee employee, List<int> selectedServices)
        {
            var salon = _dbContext.Salons.Find(employee.SalonId);

            if (salon != null && !IsWorkingHoursValid(employee.WorkingHours, salon.WorkingHours))
            {
                ModelState.AddModelError("WorkingHours", $"Working hours must be within the salon's hours: {salon.WorkingHours}");
            }

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
        [Authorize(Roles = "Admin")]
        private bool IsWorkingHoursValid(string employeeHours, string salonHours)
        {
            var employeeTimes = employeeHours.Split('-');
            var salonTimes = salonHours.Split('-');

            if (employeeTimes.Length == 2 && salonTimes.Length == 2)
            {
                var employeeStart = TimeSpan.Parse(employeeTimes[0].Trim());
                var employeeEnd = TimeSpan.Parse(employeeTimes[1].Trim());
                var salonStart = TimeSpan.Parse(salonTimes[0].Trim());
                var salonEnd = TimeSpan.Parse(salonTimes[1].Trim());

                return employeeStart >= salonStart && employeeEnd <= salonEnd;
            }

            return false;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult EditEmployee(Employee employee, List<int> selectedServices)
        {
            var salon = _dbContext.Salons.Find(employee.SalonId);

            if (salon != null && !IsWorkingHoursValid(employee.WorkingHours, salon.WorkingHours))
            {
                ModelState.AddModelError("WorkingHours", $"Working hours must be within the salon's hours: {salon.WorkingHours}");
            }

            var existingEmployee = _dbContext.Employees
                .Include(e => e.Services)
                .FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);

            if (existingEmployee != null && ModelState.IsValid)
            {
                existingEmployee.Name = employee.Name;
                existingEmployee.Surname = employee.Surname;
                existingEmployee.SalonId = employee.SalonId;
                existingEmployee.WorkingHours = employee.WorkingHours;

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
        [Authorize(Roles = "Admin")]
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
