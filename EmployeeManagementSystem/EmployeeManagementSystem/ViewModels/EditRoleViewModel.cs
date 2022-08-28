namespace EmployeeManagementSystem.ViewModels
{
    public class EditRoleViewModel
    {

        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<string> Users { get; set; } = new List<string>();

       
    }
}
