namespace EmployeeManagementSystem.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        //don't need role here cause it's gonna cause
        //duplication, pass roleId using ViewBag.
        //public string RoleId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
