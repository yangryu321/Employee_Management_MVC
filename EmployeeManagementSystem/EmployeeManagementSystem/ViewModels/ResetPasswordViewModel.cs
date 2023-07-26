namespace EmployeeManagementSystem.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string  UserId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage ="Passwords do not match" )]
        public string ConfirmPassword { get; set; }
    }
}
