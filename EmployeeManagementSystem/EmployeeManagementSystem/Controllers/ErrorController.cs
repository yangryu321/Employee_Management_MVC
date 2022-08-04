using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{statuscode}")]
        public IActionResult Index(int statuscode)
        {
            switch(statuscode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the page does not exist";
                    break;
            }

            return View("NotFound",statuscode);
        }
    }
}
