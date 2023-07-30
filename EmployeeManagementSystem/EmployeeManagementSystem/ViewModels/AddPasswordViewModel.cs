namespace EmployeeManagementSystem.ViewModels
{
    public class AddPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Add Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
