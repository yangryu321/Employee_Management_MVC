using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace EmployeeManagementSystem.Security
{
    public class CannotEditOwnRolesAndClaimsHandler :
        AuthorizationHandler<ManageAdminRoleAndClaimsRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CannotEditOwnRolesAndClaimsHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageAdminRoleAndClaimsRequirement requirement)
        {

            //get the id of the logged in user
            string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //get the id of the user being edited
            //todo
            string userBeingEditedId = httpContextAccessor.HttpContext.GetRouteData().Values["Id"].ToString();

            if (userBeingEditedId.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class MultihanderTest : AuthorizationHandler<ManageAdminRoleAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRoleAndClaimsRequirement requirement)
        {
            if (context.User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value != "test@yang.com").FirstOrDefault())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}
