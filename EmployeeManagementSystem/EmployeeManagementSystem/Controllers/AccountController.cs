using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagementSystem.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }


  


        [HttpGet]
        [AllowAnonymous]

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout() 
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(user,model.Password);


                if (result.Succeeded)
                {
                    //generate token
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    //generate email confirmation link
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token = token, userId = user.Id });

                    logger.Log(logLevel: LogLevel.Warning, confirmationLink);
                    //if the user is admin then redirect to ListUser page
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                        return RedirectToAction(actionName: "ListUsers", controllerName: "Administration");

                    //redirect to confirmation page
                    return View("EmailConfirmation");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            if(token ==null || userId ==null)
            {
                return RedirectToAction("Index", "Home");
            }

            //get the user
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return View("NotFound");
            }

            //check the token if valid
            var result = await userManager.ConfirmEmailAsync(user,token);

            if(result.Succeeded)
            {
                return View();
            }

            return View("NotFound");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            var viewmodel = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternaLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(viewmodel);

        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            model.ExternaLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //check if the user email is not confirmed, then block user login
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && user.EmailConfirmed != true && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email is not confirmed");
                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");


            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            //need a redirect url
            var redirectUrl = Url.Action("ExternalLogInCallback", "Account", new { ReturnUrl = returnUrl});
            //need properties built with that url
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);    
            //build a new challenge result
            return new ChallengeResult(provider, properties);
            
        }

        
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogInCallback(string returnUrl=null, 
            string remoteError = null)
        {
             returnUrl = returnUrl ?? Url.Content("~/");

            var viewmodel = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternaLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if(remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider:{remoteError}");
                return View("Login", viewmodel);
            }

            //get the external login info

            var info = await signInManager.GetExternalLoginInfoAsync();

            if(info == null)
            { 
                ModelState.AddModelError(string.Empty, $"Error loading external login information");
                return View("Login", viewmodel);
            }

            //it will only succeed if the theres a corresponding record in the database
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, 
                info.ProviderKey, false, true);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                var userId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    //user does't have an account 
                    if (user == null)
                    {
                        //Create an local account 
                        user = new ApplicationUser()
                        {
                            UserName = email,
                            Email = email

                        };

                        await userManager.CreateAsync(user);
                    }

                    //add user info to userLogins table
                    await userManager.AddLoginAsync(user, info);

                    //sign the user in with its local account
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);

                }
                
                if(userId !=null)
                {
                    var user = await userManager.FindByIdAsync(userId);
                    //user does't have an account 
                    if (user == null)
                    {
                        //Create an local account 
                        user = new ApplicationUser()
                        {
                            Id = userId,
                            UserName = info.Principal.FindFirst(ClaimTypes.Name).Value

                    };

                        await userManager.CreateAsync(user);
                    }

                    //add user info to userLogins table
                    await userManager.AddLoginAsync(user, info);

                    //sign the user in with its local account
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }


                return View("NotFound");
            }
        }


        [HttpGet][HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailExists(string email)
        {

            var result = await userManager.FindByEmailAsync(email);

            if(result == null)
            {
                return Json(true);
            }

            return Json($"Email {email} is already is use");


        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Forgotpassword()
        {
            
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Forgotpassword(ForgotPasswordViewModel viewModel)
        {
            
            if(ModelState.IsValid)
            {
                //check if the email is valid?
                var user = await userManager.FindByEmailAsync(viewModel.Email);
                //if the user is not null and the email is confirmed
                if(user!=null&&(await userManager.IsEmailConfirmedAsync(user)))
                {

                    //generate token
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    //generate password reset link
                    var passwordResetLink = Url.Action("ResetPassword","Account", 
                        new {token= token, userId=user.Id},Request.Scheme);

                    logger.Log(LogLevel.Warning, passwordResetLink);

                    return View("ForgotpasswordConfirmation");
                   
                }

                return View("ForgotpasswordConfirmation");

            }

            //if it is then send a link to email to reset password?
            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string userId)
        {
            var viewmodel = new ResetPasswordViewModel()
            {
                UserId = userId,
                Token = token
            };


            return View(viewmodel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(viewModel.UserId);
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.ConfirmPassword);

                    if(result.Succeeded)
                    {
                        return View("ResetPassworConfirmation");

                    }

                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(viewModel);
                    
                }
            }

            return View(viewModel);
        }
    }
}
