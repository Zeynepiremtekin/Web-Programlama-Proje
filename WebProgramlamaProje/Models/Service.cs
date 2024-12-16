using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Sabit hizmet adı

        [Required(ErrorMessage = "Süre zorunludur.")]
        public int Duration { get; set; } // Süre (dakika cinsinden)

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0, 10000, ErrorMessage = "Fiyat 0 ile 10000 arasında olmalıdır.")]
        public decimal Price { get; set; } // Fiyat
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
