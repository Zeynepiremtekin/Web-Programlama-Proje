using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebProgramlamaProje.Controllers
{
    [Authorize]
    public class LogoutController : Controller
    {
        // Kullanıcı çıkışı
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // Kullanıcı oturumunu kapat
            return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
        }
    }
}

