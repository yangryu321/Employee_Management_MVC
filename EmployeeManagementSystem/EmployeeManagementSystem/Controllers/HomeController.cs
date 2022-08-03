using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace EmployeeManagementSystem.Controllers
{
    
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment)
        {
            this.employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        [Route("")]
        [Route("~/")]
        public IActionResult Index()
        {
            var model = employeeRepository.GetAll();
            return View(model);
        }

        [Route("Try")]
        public string Try()
        {
            return employeeRepository.Get(1).Name.ToString();
        }

        
        [Route("{id?}")]
        public IActionResult Details(int? id)
        {
            Employee employee = employeeRepository.Get(id.Value);

            //if the employee is not found then direct the user to NotFound page
            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("NotFound", id.Value);
            }


            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                employee = employee,
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
            if(ModelState.IsValid)
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
        public IActionResult Edit(EmployeeEditViewModel employeeEditViewModel)
        {
            if(ModelState.IsValid)
            {
                Employee employee = employeeRepository.Get(employeeEditViewModel.Id);
                employee.Name = employeeEditViewModel.Name;
                employee.Email = employeeEditViewModel.Email;
                employee.Department = employeeEditViewModel.Department;

                //if user uploads a photo
                if(employeeEditViewModel.Photo != null)
                {

                    //if the user has a existing photo then delete it
                    if(employeeEditViewModel.ExistingPhotoPath != null)
                    {
                        var filepath = Path.Combine(hostingEnvironment.WebRootPath, "images", employeeEditViewModel.ExistingPhotoPath);
                        System.IO.File.Delete(filepath);
                       
                    }

                    //upload the new photo
                    employee.Photopath = ProcessUploadFile(employeeEditViewModel);
                }

                employeeRepository.Update(employee);
                return RedirectToAction("Index");

            }

            return View();
        }
    }
}
