using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class AboutController : Controller
    {
        // This action method returns the About view
        public IActionResult Index()
        {
            return View();
        }
    }
}
