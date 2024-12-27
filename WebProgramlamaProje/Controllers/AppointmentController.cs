using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AppointmentController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 1. Randevu Oluşturma Sayfası - GET
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            FillDropdownLists();
            return View();
        }

        // 2. Salon Seçimine Göre Çalışanları Listeleme (AJAX)
        public JsonResult GetEmployees(int salonId)
        {
            var employees = _dbContext.Employees
                .Where(e => e.SalonId == salonId)
                .Select(e => new
                {
                    e.EmployeeId,
                    FullName = e.Name + " " + e.Surname
                })
                .ToList();

            return Json(employees);
        }

        // 3. Çalışan Seçimine Göre Hizmetleri Listeleme (AJAX)
        public JsonResult GetServices(int employeeId)
        {
            var services = _dbContext.Employees
                .Include(e => e.Services)
                .Where(e => e.EmployeeId == employeeId)
                .SelectMany(e => e.Services)
                .Select(s => new
                {
                    s.ServiceId,
                    s.Name,
                    s.Duration
                })
                .Distinct()
                .ToList();

            return Json(services);
        }

        // 4. Randevu Oluşturma - POST
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment appointment)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Please sign in.");
                FillDropdownLists();
                return View(appointment);
            }

            // Kullanıcı ID'yi ata
            appointment.UserId = int.Parse(userId);

            // Seçilen çalışanı getir
            var employee = _dbContext.Employees
                .Include(e => e.Services)
                .FirstOrDefault(e => e.EmployeeId == appointment.EmployeeId);

            if (employee == null)
            {
                ModelState.AddModelError("", "The selected employee could not be found.");
                FillDropdownLists();
                return View(appointment);
            }

            // Çalışma saatleri kontrolü
            var workingHours = employee.WorkingHours.Split('-');
            var workingStart = TimeSpan.Parse(workingHours[0].Trim());
            var workingEnd = TimeSpan.Parse(workingHours[1].Trim());

            var appointmentTime = appointment.AppointmentDateTime.TimeOfDay;
            var selectedService = _dbContext.Services.FirstOrDefault(s => s.ServiceId == appointment.ServiceId);

            if (selectedService == null)
            {
                ModelState.AddModelError("", "Service not found");
                FillDropdownLists();
                return View(appointment);
            }

            var appointmentEndTime = appointmentTime + TimeSpan.FromMinutes(selectedService.Duration);

            // Çalışma saatleri dışı kontrol
            if (appointmentTime < workingStart || appointmentEndTime > workingEnd)
            {
                ModelState.AddModelError("", $"This employee works between {employee.WorkingHours}.");
                FillDropdownLists();
                return View(appointment);
            }

            // Çakışma kontrolü
            var isConflict = _dbContext.Appointments.Any(a =>
                a.EmployeeId == appointment.EmployeeId &&
                a.AppointmentDateTime < appointment.AppointmentDateTime.AddMinutes(selectedService.Duration) &&
                a.AppointmentDateTime.AddMinutes(a.Service.Duration) > appointment.AppointmentDateTime &&
                (a.Status == "Approved" || a.Status == "Pending") // Onaylı ya da pending randevular çakışacak
            );

            if (isConflict)
            {
                ModelState.AddModelError("", "The selected time range is full or unavailable. Please choose another time.");
                FillDropdownLists();
                return View(appointment);
            }

            if (ModelState.IsValid)
            {
                appointment.Status = "Pending"; // Onay bekliyor olarak ayarla
                _dbContext.Appointments.Add(appointment);
                _dbContext.SaveChanges();
                return RedirectToAction("MyAppointments");
            }

            FillDropdownLists();
            return View(appointment);
        }

        // 5. Kullanıcının Randevularını Görüntüleme
        [Authorize]
        public IActionResult MyAppointments()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var appointments = _dbContext.Appointments
                .Include(a => a.Salon)
                .Include(a => a.Service)
                .Include(a => a.Employee)
                .Where(a => a.UserId == int.Parse(userId))
                .ToList();

            return View(appointments);
        }

        // Admin Randevuları Görüntüleme
        [Authorize(Roles = "Admin")]
        public IActionResult ViewAdminAppointments()
        {
            var appointments = _dbContext.Appointments
                .Include(a => a.User)
                .Include(a => a.Salon)
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .ToList();

            return View("ViewAdminAppointments", appointments);
        }

        // Admin Randevuyu Onaylama
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Confirm(int id)
        {
            var appointment = _dbContext.Appointments.Find(id);
            if (appointment != null)
            {
                appointment.Status = "Approved"; // Durumu "Approved" olarak ayarla
                _dbContext.SaveChanges();
            }
            return RedirectToAction("ViewAdminAppointments");
        }

        // Admin Randevuyu Reddetme
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Reject(int id)
        {
            var appointment = _dbContext.Appointments.Find(id);
            if (appointment != null)
            {
                appointment.Status = "Rejected"; // Durumu "Rejected" olarak ayarla
                _dbContext.SaveChanges();
            }
            return RedirectToAction("ViewAdminAppointments");
        }



        // Admin Randevuyu Silme
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var appointment = _dbContext.Appointments.Find(id);
            if (appointment != null)
            {
                _dbContext.Appointments.Remove(appointment);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("ViewAdminAppointments");
        }

        private void FillDropdownLists()
        {
            ViewBag.Salons = new SelectList(_dbContext.Salons, "SalonId", "Name");
        }
    }
}
