using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Service name is required.")]
        [MaxLength(100, ErrorMessage = "Service name cannot exceed 100 characters.")]
        public string Name { get; set; } 

        [Required(ErrorMessage = "Duration is required.")]
        public int Duration { get; set; } 

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000.")]
        public decimal Price { get; set; } 

        public string? ImagePath { get; set; } 
        public string? VideoUrl { get; set; } 
        public string? Description { get; set; } 

        public List<Employee> Employees { get; set; } = new List<Employee>(); 
    }
}
