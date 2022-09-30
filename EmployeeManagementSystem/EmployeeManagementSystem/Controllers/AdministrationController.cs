using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    [Route("[controller]/[action]")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
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


        [HttpGet]
        public async Task<ActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMessage = "The role does not exist";
                return View("NotFound");
            }

            EditRoleViewModel model = new EditRoleViewModel()
            {
                RoleId = id,
                RoleName = role.Name,

            };
                
            var users = userManager.Users.ToList();

            foreach(var user in users)
            {
                if(await userManager.IsInRoleAsync(user,role.Name))
                    model.Users.Add(user.UserName);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                //TODO
           
                var role = await roleManager.FindByIdAsync(model.RoleId);
                if (role == null)
                {
                    ViewBag.ErrorMessage = "The role does not exist";
                    return View("NotFound");
                }

                role.Name = model.RoleName;

                var result = await roleManager.UpdateAsync (role);

                if(result.Succeeded)
                {
                    return RedirectToAction(actionName: "ListRoles", controllerName: "Administration");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            List<UserRoleViewModel> roles = new List<UserRoleViewModel>();
            var role = await roleManager.FindByIdAsync(roleId);
            ViewBag.RoleId = roleId;
            var users = userManager.Users.ToList();


            if(role is null)
            {
                ViewBag.ErrorMessage = $"Role with Id {roleId} can not be found";
                return View("NotFound");
            }

            foreach(var user in users)
            {
                UserRoleViewModel model = new UserRoleViewModel();
                model.UserId = user.Id;
                model.UserName = user.UserName;
                //if the user is in that role, then make IsSelected true
                if(await userManager.IsInRoleAsync(user,role.Name))
                {
                    model.IsSelected = true;
                }
                else
                {
                    model.IsSelected = false;
                }
               
                roles.Add(model);
             
            }

            return View(roles);
        }


        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> models, string roleId)
        {
            if(ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(roleId);

                foreach (var model in models)
                {
                    //reset all roles first, because if the roles are not reset, there will be duplication of data. Considering Tomo is already checked for this role
                    //and no change is made and you are checking the IsSelected property with a foreach roop. Since tomo is already checked for this role so the method 
                    //has already been called and will be called again this way
                    var user = await userManager.FindByIdAsync(model.UserId);

                    //need error check?
                    await userManager.RemoveFromRoleAsync(user, role.Name);

                    if (model.IsSelected)
                    {
                        //need error check?
                        await userManager.AddToRoleAsync(user, role.Name);
                    }



                }
                return RedirectToAction(actionName: "EditRole", controllerName: "Administration", new { Id = roleId });

            }
            return View();
        }



    }
}
