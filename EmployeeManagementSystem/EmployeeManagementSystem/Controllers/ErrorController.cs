using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
            logger.LogDebug(1, "NLog injected into HomeController");
        }
        [Route("Error/{statuscode}")]
        public IActionResult Index(int statuscode)
        {
            logger.LogInformation("Hello, this is the index!");
            switch (statuscode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the page does not exist";
                    break;
            }

            return View("NotFound",statuscode);
        }
    }
}
