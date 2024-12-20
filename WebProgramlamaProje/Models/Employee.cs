using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "You must select a salon.")]
        public int SalonId { get; set; }

        public Salon? Salon { get; set; } 

        public List<Service> Services { get; set; } = new List<Service>();

        [Required(ErrorMessage = "Working hours are required.")]
        public string WorkingHours { get; set; }
    }
}
