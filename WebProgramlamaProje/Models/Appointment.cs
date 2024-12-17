using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProgramlamaProje.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; } // Randevu ID'si (Primary Key)

        [Required]
        public int UserId { get; set; } // Randevuyu alan kullanıcı ID'si
        public User? User { get; set; } // Kullanıcı ile ilişki

        [Required]
        public int SalonId { get; set; } // Seçilen salon ID'si
        public Salon? Salon { get; set; } // Salon ile ilişki

        [Required]
        public int EmployeeId { get; set; } // Seçilen çalışan ID'si
        public Employee? Employee { get; set; } // Çalışan ile ilişki

        [Required]
        public int ServiceId { get; set; } // Seçilen hizmet ID'si
        public Service? Service { get; set; } // Hizmet ile ilişki

        [Required]
        public DateTime AppointmentDateTime { get; set; } // Randevu tarihi ve saati

        public bool IsConfirmed { get; set; } = false; // Randevu onay durumu (varsayılan: onaylanmadı)
    }
}
