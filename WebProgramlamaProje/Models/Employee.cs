using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Adı doldurmanız zorunludur.")]
        [MaxLength(50, ErrorMessage = "Ad 50 karakterden uzun olamaz.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyadı doldurmanız zorunludur.")]
        [MaxLength(50, ErrorMessage = "Soyad 50 karakterden uzun olamaz.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Salon seçmeniz zorunludur.")]
        public int SalonId { get; set; }

        public Salon? Salon { get; set; } // Çalışanın bağlı olduğu salon

        public List<Service> Services { get; set; } = new List<Service>();
    }
}
