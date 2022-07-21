using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }
        public IActionResult Index()
        {
            var model = employeeRepository.GetAll();
            return View(model);
        }

        public string Try()
        {
            return employeeRepository.Get(1).Name.ToString();
        }

        public IActionResult Details()
        {
            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                employee = employeeRepository.Get(1),
                Title = "Details Page"

            };

            return View(model);
        }
    }
}
