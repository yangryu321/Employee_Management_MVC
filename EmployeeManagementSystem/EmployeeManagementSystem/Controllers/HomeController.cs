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
            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                employee = employeeRepository.Get(id??1),
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
                string uniquefilename = null; 
                if(model.Photo!=null)
                {
                    string uploadfolder =  Path.Combine(hostingEnvironment.WebRootPath, "images");
                    uniquefilename = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filepath = Path.Combine(uploadfolder, uniquefilename);

                    using(FileStream fs = new FileStream(filepath, FileMode.Create))
                    {
                        model.Photo.CopyTo(fs);
                    }
                    

                }
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
    }
}
