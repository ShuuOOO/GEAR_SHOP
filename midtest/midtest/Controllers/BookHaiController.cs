using Microsoft.AspNetCore.Mvc;

namespace midtest.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
