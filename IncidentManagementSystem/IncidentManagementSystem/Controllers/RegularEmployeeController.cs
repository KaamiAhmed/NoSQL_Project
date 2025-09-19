using Microsoft.AspNetCore.Mvc;

namespace IncidentManagementSystem.Controllers
{
    public class RegularEmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
