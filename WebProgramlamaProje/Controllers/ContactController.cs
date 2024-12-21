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

                return RedirectToAction("ContactUs", "Home"); // Aynı sayfaya yönlendir
            }

            return View("ContactUs", "Home");
        }

        [HttpPost]
        public IActionResult DeleteMessage(int id)
        {
            var message = _dbContext.ContactMessages.Find(id);
            if (message != null)
            {
                _dbContext.ContactMessages.Remove(message);
                _dbContext.SaveChanges();
            }

            // Silme işleminden sonra Admin/ContactMessages sayfasına yönlendirme
            return RedirectToAction("ContactMessages", "Admin");
        }


    }
}
