using System.Security.Claims;

namespace EmployeeManagementSystem.ViewModels
{
    public static class ClaimStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Delete Role","Delete Role"),
            new Claim("Edit Role", "Edit ROle"),
            new Claim("Create Role", "Create Role")
        };

    }
}
