using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ContactController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult SubmitMessage(ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                _dbContext.ContactMessages.Add(contactMessage);
                _dbContext.SaveChanges();

                // TempData ile başarı mesajını taşırız
                TempData["Message"] = "Your message has been sent successfully.";
                TempData["MessageType"] = "success"; // Mesaj türü başarılı

                return RedirectToAction("ContactUs", "Home"); // Aynı sayfaya yönlendir
            }

            // TempData ile hata mesajını taşırız
            TempData["Message"] = "An error occurred. Please try again.";
            TempData["MessageType"] = "error"; // Mesaj türü hata

            return View("ContactUs", "Home");
        }
    }
}
