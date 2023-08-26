using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagementSystem.Controllers
{

    [Authorize(Roles = "Admin, Super Admin")]
    
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }


        [HttpGet]
        [Authorize(Policy ="Create Role")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [Authorize(Policy = "Create Role")]
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

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

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
        public IActionResult ListUsers()
        {
            //todo Might need to delete the database and build it from scrach
            var users = userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<ActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
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

            foreach (var user in users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                    model.Users.Add(user.UserName);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO

                var role = await roleManager.FindByIdAsync(model.RoleId);
                if (role == null)
                {
                    ViewBag.ErrorMessage = "The role does not exist";
                    return View("NotFound");
                }

                role.Name = model.RoleName;

                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction(actionName: "ListRoles", controllerName: "Administration");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

            }

            return View();
        }


        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            List<UserRoleViewModel> roles = new List<UserRoleViewModel>();
            var role = await roleManager.FindByIdAsync(roleId);
            ViewBag.RoleId = roleId;
            var users = userManager.Users.ToList();


            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with Id {roleId} can not be found";
                return View("NotFound");
            }

            foreach (var user in users)
            {
                UserRoleViewModel model = new UserRoleViewModel();
                model.UserId = user.Id;
                model.UserName = user.UserName;
                //if the user is in that role, then make IsSelected true
                if (await userManager.IsInRoleAsync(user, role.Name))
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
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> models, string roleId)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(roleId);

                foreach (var model in models)
                {
                    //reset all roles first, because if the roles are not reset, there will be duplication of data. Considering Tomo is already checked for this role
                    //and no change is made and you are checking the IsSelected property with a foreach roop. Since tomo is already checked for this role so the method 
                    //has already been called and will be called again
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


        [HttpPost]
        [Authorize(Policy = "Delete Role")]
        //delete role
        public async Task<IActionResult> Delete(string id)
        {

            var role = await roleManager.FindByIdAsync(id);

            if (role != null)
            {
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("ListRoles");
            }

            return View("NotFound");
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            var roles = await userManager.GetRolesAsync(user);
            var claims = await userManager.GetClaimsAsync(user);

            var viewmodel = new EditUserViewModel()
            {
                Id = Id,
                UserName = user.UserName,
                Email = user.Email,
                UserClaims = claims.Select(c => c.Type +" : "+c.Value).ToList(),
                UserRoles = (List<string>)roles
            };


            //Todo make the edituser page prettier  
            if (user != null)
                return View(viewmodel);
            else
                return View("NotFound");
        }


        
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(viewModel.Id);
                user.UserName = viewModel.UserName;
                user.Email = viewModel.Email;
              
                //var roles = viewModel.UserRoles;
                //await userManager.AddToRolesAsync(user, roles);
               

                //var claims = viewModel.UserClaims;
                //foreach(var claim in claims)
                //{
                //    var splitClaim = claim.Split(':');
                //    if (splitClaim.Length == 2)
                //    {
                //        var claimType = splitClaim[0].Trim();
                //        var claimValue = splitClaim[1].Trim();
                //        var newclaim = new Claim(claimType, claimValue);

                //        // Add the claim to the user
                //        await userManager.AddClaimAsync(user, newclaim);
                //    }

                //}

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                
            }

            return View(viewModel);
        }


        [HttpPost]
        [Authorize(Policy = "Delete User")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if(user != null)
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                    return RedirectToAction("ListUsers");
            }
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "CannotEditYourself")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ManageRoles(string Id)
        {
            //dont need to pass userId through viewdata here?
            //ViewData["Id"] = Id;
            var user = await userManager.FindByIdAsync(Id);
            var roles = roleManager.Roles.ToList();
            var viewmodel = new List<RolesInUser>();

            foreach (var role in roles)
            {
                var model = new RolesInUser();
                model.RoleId = role.Id;
                model.RoleName = role.Name;

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.IsSelected = true;

                }
                else
                {
                    model.IsSelected = false;
                }
                viewmodel.Add(model);
            }

            return View(viewmodel);

        }

        [HttpPost]
        [Authorize(Policy = "CannotEditYourself")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ManageRoles(List<RolesInUser> viewmodel, string Id)
        {
            //todo 7.15
            var user = await userManager.FindByIdAsync(Id);

            //reset all the roles first

            foreach (var role in viewmodel)
            {
                await userManager.RemoveFromRoleAsync(user, role.RoleName);
                if (role.IsSelected)
                {
                    await userManager.AddToRoleAsync(user, role.RoleName);
                }

            }



            return RedirectToAction("EditUser", new { Id = Id });

        }

        [HttpGet]
        [Authorize(Policy = "CannotEditYourself")]
      
        public async Task<IActionResult> ManageClaims(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            var existingClaims = await userManager.GetClaimsAsync(user);
            

            var models = new List<UserClaimsViewModel>();

            foreach (var claim in ClaimStore.AllClaims)
            {
                var model = new UserClaimsViewModel()
                {
                    //no need to pass Id here cause of model binding
                    //UserId = user.Id,
                    ClaimType = claim.Type,
                    IsSelected = false
                };

                //if the user has any claim in of the current claim type and the value if true
                if (existingClaims.Any(c => c.Type == claim.Type && c.Value =="true"))
                {
                    model.IsSelected = true;
                }
                models.Add(model);
            }
            return View(models);
        }

        [HttpPost]
        [Authorize(Policy = "CannotEditYourself")]
        public async Task<IActionResult> ManageClaims(List<UserClaimsViewModel> viewModel, string Id)
        {
            var user = await userManager.FindByIdAsync(Id);

            var claims = await userManager.GetClaimsAsync(user);
            await userManager.RemoveClaimsAsync(user, claims);    

            foreach (var model in viewModel)
            {
                if (model.IsSelected)
                {
                    var result = await userManager.AddClaimAsync(user, new Claim(model.ClaimType, "true"));

                    if (!result.Succeeded)
                        return View(viewModel);
                }
                else
                {
                    var result = await userManager.AddClaimAsync(user, new Claim(model.ClaimType, "false"));

                    if (!result.Succeeded)
                        return View(viewModel);
                }

            }

            return RedirectToAction("EditUser", new {Id = Id}); 
        }

    }
}

