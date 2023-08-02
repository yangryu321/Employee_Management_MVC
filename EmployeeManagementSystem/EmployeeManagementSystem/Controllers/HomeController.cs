using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace EmployeeManagementSystem.Controllers
{


    
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IDataProtector protector;

        public HomeController(IEmployeeRepository employeeRepository,
            IHostingEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposStrings dataProtectionPurposStrings)
        {
            this.employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposStrings.EmployeeRouteDPSring);
                
        }

       

        [AllowAnonymous]
        public IActionResult Index()
        {
            //var model = employeeRepository.GetAll().
            //    Select(c =>
            //    {
            //        c.EncryptedId = protector.Protect(c.Id.ToString());
            //        return c;
            //    });
            //return View(model);

            return View("MainPage");
        }

        [AllowAnonymous]
        public IActionResult List()
        {
            var model = employeeRepository.GetAll().
               Select(c =>
               {
                   c.EncryptedId = protector.Protect(c.Id.ToString());
                   return c;
               });
            
            return View("Index",model);
        }
        //[Route("Try")]
        //public string Try()
        //{
        //    return employeeRepository.Get(1).Name.ToString();
        //}



        public IActionResult Details(string id)
        {
            int decrypedid = Convert.ToInt32(protector.Unprotect(id));

            //if the employee doesn't exist then redirect user to NotFound page
            Employee employee = employeeRepository.Get(decrypedid);

            if (employee == null)
                return View("EmployeeNotFound", decrypedid);

            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                Employee = employee,
                Title = "Details Page"

            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniquefilename = ProcessUploadFile(model);
                Employee employee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    Photopath = uniquefilename
                };

                employeeRepository.Add(employee);
                return RedirectToAction("Details", new { ID = employee.Id });

            }
            return View();
        }

        private string ProcessUploadFile(EmployeeCreateViewModel model)
        {
            string uniquefilename = null;
            if (model.Photo != null)
            {
                string uploadfolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniquefilename = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filepath = Path.Combine(uploadfolder, uniquefilename);

                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    model.Photo.CopyTo(fs);
                }


            }

            return uniquefilename;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Employee employee = employeeRepository.Get(id);

            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.Photopath

            };


            return View(employeeEditViewModel);
        }


        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = employeeRepository.Get(model.Id);

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                //if the user uploaded a photo
                if (model.Photo != null)
                {
                    //if the employee already has a photo then delete it
                    if (model.ExistingPhotoPath != null)
                    {
                        var filepath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filepath);
                    }
                    employee.Photopath = ProcessUploadFile(model);

                }

                employeeRepository.Update(employee);
                return RedirectToAction("Details", new { Id = model.Id });
            }

            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var employee = employeeRepository.Get(id);
            if(employee != null)
            {
                employeeRepository.Delete(id);
                return RedirectToAction("Index");
            }
            return View("NotFound");
        }
    }
    
}
