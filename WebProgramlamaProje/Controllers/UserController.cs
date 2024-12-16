using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Yeni kullanıcı kaydı oluşturmak için sayfa
        [AllowAnonymous]
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        public IActionResult UserProfile()
        {
            return View();
        }

        // Yeni kullanıcı kaydı oluşturma işlemi
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(User receivedUser)
        {
            // Aynı e-posta ile başka bir kullanıcı olup olmadığını kontrol et
            var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == receivedUser.Email);
            if (existingUser != null)
            {
                TempData["Error"] = "Bu e-posta adresi zaten kayıtlı.";
                return View(receivedUser); // Kullanıcıyı form ekranına geri döndür
            }

            // Yeni kullanıcının varsayılan rolünü ve diğer özelliklerini ayarla
            receivedUser.Role = "User";
            receivedUser.UserId = 0; // Veri tabanı otomatik artırırsa manuel değer atamaya gerek yok

            if (ModelState.IsValid)
            {
                _dbContext.Users.Add(receivedUser);
                _dbContext.SaveChanges();
                return RedirectToAction("LoginPage", "Login");
            }
            else
            {
                TempData["Warning"] = "Kayıt başarısız. Lütfen bilgilerinizi kontrol edin.";
                return View(receivedUser);
            }
        }
    }
}
