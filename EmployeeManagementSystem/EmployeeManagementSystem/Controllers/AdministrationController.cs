using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    [Route("[controller]/[action]")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {

            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole()
                {
                    Name = model.RoleName
                };

                var result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                    return RedirectToAction(actionName: "ListRoles", controllerName: "Administration");

                foreach(var error in result.Errors)
                    ModelState.AddModelError("",error.Description);

            }

            return View();
        } 


        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }




    }
}
