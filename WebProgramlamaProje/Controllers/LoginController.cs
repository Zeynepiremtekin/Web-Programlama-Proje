using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebProgramlamaProje.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebProgramlamaProje.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public LoginController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Login sayfasını görüntüleme
        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }

        // Login işlemi
        [HttpPost]
        public async Task<IActionResult> LoginPage(User loginUser)
        {
            // Kullanıcıyı e-posta adresine göre arama
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == loginUser.Email);

            // Kullanıcı bulundu ve şifre eşleşiyor mu kontrolü
            if (user != null && user.Password == loginUser.Password)
            {
                // Kullanıcı giriş bilgileri doğruysa, claim listesi oluşturma
                var claims = new List<Claim>
{
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
};

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                    });

                // Rolüne göre yönlendirme
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }

            // Giriş başarısızsa hata mesajı
            TempData["Error"] = "Invalid email or password.";
            return View();
        }

        // Logout işlemi
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoginPage", "Login");
        }
    }
}
