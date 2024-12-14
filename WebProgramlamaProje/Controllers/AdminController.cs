using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        // 1. Salonları Listeleme
        public IActionResult ViewSalons()
        {
            var salons = _dbContext.Salons.ToList(); // Tüm salonları getir
            return View(salons); // View'e gönder
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
                _dbContext.Salons.Add(salon); // Yeni salonu ekle
                _dbContext.SaveChanges(); // Değişiklikleri kaydet
                return RedirectToAction("ViewSalons"); // Salonlar listesine yönlendir
            }

            return View(salon); // Hata durumunda aynı formu göster
        }

        // 4. Salon Düzenleme - GET
        [HttpGet]
        public IActionResult EditSalon(int id)
        {
            var salon = _dbContext.Salons.Find(id); // İlgili salonu bul
            if (salon == null)
            {
                return NotFound(); // Eğer salon bulunamazsa 404 döndür
            }

            return View(salon); // Düzenleme formunu göster
        }

        // 5. Salon Düzenleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSalon(Salon salon)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Salons.Update(salon); // Salonu güncelle
                _dbContext.SaveChanges(); // Değişiklikleri kaydet
                return RedirectToAction("ViewSalons"); // Salonlar listesine yönlendir
            }

            return View(salon); // Hata durumunda aynı formu göster
        }

        // 6. Salon Silme
        [HttpPost]
        public IActionResult DeleteSalon(int id)
        {
            // Önce o salona bağlı çalışanları sil
            var employees = _dbContext.Employees.Where(e => e.SalonId == id).ToList();
            if (employees.Any())
            {
                _dbContext.Employees.RemoveRange(employees);
            }

            // Daha sonra salonu sil
            var salon = _dbContext.Salons.Find(id);
            if (salon != null)
            {
                _dbContext.Salons.Remove(salon);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("ViewSalons");
        }

        public IActionResult ViewEmployees()
        {
            // Tüm çalışanları ve ilgili salon bilgilerini sorgula
            var employees = _dbContext.Employees
                .Include(e => e.Salon) // Her çalışanın salon bilgisini dahil et
                .ToList();

            return View(employees); // Görünüme gönder
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            // Salon listesini ViewBag ile gönder
            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEmployee(Employee employee)
        {
            if (ModelState.IsValid) // Form doğrulamasını kontrol et
            {
                _dbContext.Employees.Add(employee);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewEmployees", new { salonId = employee.SalonId });
            }

            // Hata durumunda salon listesini yeniden gönder
            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                })
                .ToList();

            return View(employee);
        }

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var employee = _dbContext.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Employees.Update(employee);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewEmployees", new { salonId = employee.SalonId });
            }

            // Hata durumunda salon listesini yeniden doldur
            ViewBag.SalonList = _dbContext.Salons
                .Select(s => new SelectListItem
                {
                    Value = s.SalonId.ToString(),
                    Text = s.Name
                })
                .ToList();

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

            return RedirectToAction("ViewEmployees", new { salonId = employee.SalonId });
        }

    }
}
