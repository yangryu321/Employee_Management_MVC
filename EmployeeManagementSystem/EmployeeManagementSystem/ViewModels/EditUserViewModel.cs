namespace EmployeeManagementSystem.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        public List<string> UserRoles { get; set; } = new List<string>();
        public List<string> UserClaims { get; set; } = new List<string>();
    }
}
