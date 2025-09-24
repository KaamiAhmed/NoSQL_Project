using Microsoft.AspNetCore.Mvc;

namespace IncidentManagementSystem.Controllers
{
    public class ServiceDeskEmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
