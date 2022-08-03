using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    
    public class ErrorController : Controller
    {
        [Route("Error/{statuscode}")]
        public IActionResult Index(int statuscode)
        {
            switch (statuscode)
            {
                case 404: 
                    ViewBag.ErrorMessage = "The page is not found";
                    break;
            }

            return View("NotFound");
        }
    }
}
