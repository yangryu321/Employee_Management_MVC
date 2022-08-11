

using EmployeeManagementSystem.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailExists", controller: "Account")]
        [ValidEmailDomianValidation(alloweddomain:"yang.com",ErrorMessage = "Email domin must be yang.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match, please try again")]
        public string ConfirmPassword { get; set; }
    }
}
