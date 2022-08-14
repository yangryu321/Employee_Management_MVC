using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
           
        }

        [Route("Error/{statuscode}")]
        public IActionResult Index(int statuscode)
        {
            
            switch (statuscode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the page does not exist";
                    logger.LogInformation("404 error, page not found");
                    break;
            }

            return View("NotFound",statuscode);
        }
    }
}
