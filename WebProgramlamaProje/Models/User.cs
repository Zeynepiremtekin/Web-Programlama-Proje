using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Lütfen adınızı giriniz.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen soyadınızı giriniz.")]
        [MaxLength(50)]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen e-postanızı giriniz.")]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
        [MinLength(8, ErrorMessage = "Şifre en az 8 karakter uzunluğunda olmalı.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";
    }
}
