using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50,ErrorMessage ="Name can be no longer than 50 charactors")]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        ErrorMessage = "Invalid email")]
        public string Email { get; set; }
        [Required]
        public Dpt? Department { get; set; }
        public string? Photopath { get; set; }
    }
}
