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

        // Yeni kullanıcı kaydı oluşturma işlemi
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(User receivedUser)
        {
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
