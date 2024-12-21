using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProgramlamaProje.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; } 

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; } 
        public User? User { get; set; } 

        [Required(ErrorMessage = "Salon ID is required.")]
        public int SalonId { get; set; } 
        public Salon? Salon { get; set; } 

        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmployeeId { get; set; } 
        public Employee? Employee { get; set; } 

        [Required(ErrorMessage = "Service ID is required.")]
        public int ServiceId { get; set; } 
        public Service? Service { get; set; } 

        [Required(ErrorMessage = "Appointment date and time are required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid date and time.")]
        public DateTime AppointmentDateTime { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
