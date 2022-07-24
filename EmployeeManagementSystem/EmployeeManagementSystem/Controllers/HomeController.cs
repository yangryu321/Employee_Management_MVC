using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
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
    }
}
