using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class ApiUserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ApiUserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Kullanıcı Bilgilerini Getir
        [HttpGet("profile")]
        public async Task<ActionResult<User>> GetProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == int.Parse(userId));

            if (user == null)
                return NotFound();

            return user;
        }

        // Kullanıcı Bilgilerini ve Şifreyi Güncelle
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] User updatedUser)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || updatedUser.UserId != int.Parse(userId))
                return Unauthorized();

            var user = await _dbContext.Users.FindAsync(updatedUser.UserId);

            if (user == null)
                return NotFound();

            // Kullanıcı bilgilerini güncelle
            user.Name = updatedUser.Name;
            user.Surname = updatedUser.Surname;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password; // Şifreyi de güncelle

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Users.Any(u => u.UserId == updatedUser.UserId))
                    return NotFound();
                throw;
            }

            return Ok(new { message = "Bilgiler başarıyla güncellendi." });
        }

        // Kullanıcı Hesabını Sil
        [HttpDelete("profile")]
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _dbContext.Users.FindAsync(int.Parse(userId));

            if (user == null)
                return NotFound();

            // Kullanıcıyı veritabanından sil
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            // Oturumu kapat
            await HttpContext.SignOutAsync();

            return Ok(new { message = "Your account has been successfully deleted." });
        }

    }
}
