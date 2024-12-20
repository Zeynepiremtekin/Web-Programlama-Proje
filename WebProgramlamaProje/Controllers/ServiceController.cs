using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Models;
using System.IO;

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
        public IActionResult CreateService(Service service, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string imagePath = SaveImage(imageFile);
                    service.ImagePath = imagePath;
                }

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
            if (service == null)
                return NotFound();

            return View(service);
        }

        [HttpPost]
        public IActionResult EditService(Service service, IFormFile? imageFile)
        {
            // Mevcut servisi bul
            var existingService = _dbContext.Services.Find(service.ServiceId);

            if (existingService != null)
            {
                // Temel bilgileri güncelle
                existingService.Name = service.Name;
                existingService.Duration = service.Duration;
                existingService.Price = service.Price;
                existingService.Description = service.Description;
                existingService.VideoUrl = service.VideoUrl;

                // Eğer yeni bir resim seçilmişse
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Yeni resmi kaydet
                    string newImagePath = SaveImage(imageFile);
                    existingService.ImagePath = newImagePath; // Yeni resim yolunu güncelle
                }
                // Yeni resim seçilmezse mevcut resim korunur

                // Değişiklikleri kaydet
                _dbContext.Services.Update(existingService);
                _dbContext.SaveChanges();

                return RedirectToAction("ViewServices");
            }

            return View(service); // Servis bulunamazsa geri döner
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

        private string SaveImage(IFormFile imageFile)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                return "/images/" + uniqueFileName;
            }
            catch
            {
                return "/images/default.png";
            }
        }
    }
}
