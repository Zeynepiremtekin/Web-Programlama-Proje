using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Salon
    {
        [Key]
        public int SalonId { get; set; } // Birincil Anahtar

        [Required(ErrorMessage = "Salon adı gereklidir.")]
        [MaxLength(100, ErrorMessage = "Salon adı 100 karakterden uzun olamaz.")]
        public string Name { get; set; } = string.Empty; // Salonun Adı

        [Required(ErrorMessage = "Adres gereklidir.")]
        [MaxLength(200, ErrorMessage = "Adres 200 karakterden uzun olamaz.")]
        public string Address { get; set; } = string.Empty; // Salonun Adresi

        [Required(ErrorMessage = "Telefon numarası gereklidir.")]
        [MaxLength(20, ErrorMessage = "Telefon numarası 20 karakterden uzun olamaz.")]
        public string PhoneNumber { get; set; } = string.Empty; // Telefon Numarası
        [Required(ErrorMessage = "Çalışma saatleri gereklidir.")]
        [MaxLength(50, ErrorMessage = "Çalışma saatleri 50 karakterden uzun olamaz.")]
        public string WorkingHours { get; set; } // Çalışma Saatleri (Varsayılan değer)
        public string? ImagePath { get; set; } // Resim Yolu
        public List<Employee>? Employees { get; set; }
    }
}
