using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Salon
    {
        [Key]
        public int SalonId { get; set; } 
        [Required(ErrorMessage = "Salon name is required.")]
        [MaxLength(100, ErrorMessage = "Salon name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string PhoneNumber { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Working hours are required.")]
        [MaxLength(50, ErrorMessage = "Working hours cannot exceed 50 characters.")]
        public string WorkingHours { get; set; } 

        public string? ImagePath { get; set; } 

        public List<Employee>? Employees { get; set; } 
    }
}
