using Microsoft.AspNetCore.Mvc;

namespace AINotesHub.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
