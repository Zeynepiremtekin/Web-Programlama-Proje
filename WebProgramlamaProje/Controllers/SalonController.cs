using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;

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

        // Salonları Listeleme
        public IActionResult ViewSalons()
        {
            var salons = _dbContext.Salons.ToList();

            // Null veya boş liste kontrolü
            if (salons == null || !salons.Any())
            {
                ViewBag.Message = "No salons available.";
                return View(new List<Salon>());
            }

            return View(salons);
        }

        // Yeni Salon Ekleme - GET
        [HttpGet]
        public IActionResult CreateSalon()
        {
            return View();
        }

        // Yeni Salon Ekleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSalon(Salon salon, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string imagePath = SaveImage(imageFile);
                    salon.ImagePath = imagePath;
                }

                _dbContext.Salons.Add(salon);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewSalons");
            }

            return View(salon);
        }

        // Salon Düzenleme - GET
        [HttpGet]
        public IActionResult EditSalon(int id)
        {
            var salon = _dbContext.Salons.Find(id);
            if (salon == null)
            {
                TempData["Error"] = "Salon not found.";
                return RedirectToAction("ViewSalons");
            }

            return View(salon);
        }

        // Salon Düzenleme - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSalon(Salon salon, IFormFile? imageFile)
        {
            // Mevcut salonu bul
            var existingSalon = _dbContext.Salons.Find(salon.SalonId);

            if (existingSalon != null)
            {
                // Temel bilgileri güncelle
                existingSalon.Name = salon.Name;
                existingSalon.Address = salon.Address;
                existingSalon.PhoneNumber = salon.PhoneNumber;
                existingSalon.WorkingHours = salon.WorkingHours;

                // Eğer yeni bir resim seçilmişse işle
                if (imageFile != null && imageFile.Length > 0)
                {
                    string newImagePath = SaveImage(imageFile); // Yeni resmi kaydet
                    existingSalon.ImagePath = newImagePath;     // Yeni yol veritabanında güncellensin
                }
                // Eğer yeni resim seçilmediyse mevcut resim korunur

                // Veritabanını güncelle
                _dbContext.Salons.Update(existingSalon);
                _dbContext.SaveChanges();
                return RedirectToAction("ViewSalons");
            }

            return View(salon);
        }

        // Resim Kaydetme Metodu
        private string SaveImage(IFormFile imageFile)
        {
            try
            {
                // "wwwroot/images" klasörünü belirle
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Klasör yoksa oluştur
                }

                // Orijinal dosya adını al
                var fileName = Path.GetFileName(imageFile.FileName);

                // Dosya adının çakışmasını önlemek için kontrol et
                var filePath = Path.Combine(uploadsFolder, fileName);
                int count = 1;
                while (System.IO.File.Exists(filePath)) // Eğer aynı isimde dosya varsa
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    var extension = Path.GetExtension(fileName);
                    fileName = $"{fileNameWithoutExtension}_{count}{extension}"; // Yeni bir isim oluştur (örneğin: "image_1.png")
                    filePath = Path.Combine(uploadsFolder, fileName);
                    count++;
                }

                // Dosyayı belirlenen yola kaydet
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                // Dosya yolu olarak "/images" alt klasörünü döndür
                return "/images/" + fileName;
            }
            catch (Exception ex)
            {
                // Hata durumunda varsayılan bir resim döndür
                return "/images/default.png";
            }
        }
    }
}
