using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }  

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
        public List<AuthenticationScheme> ExternaLogins { get; set; }
    }
}
