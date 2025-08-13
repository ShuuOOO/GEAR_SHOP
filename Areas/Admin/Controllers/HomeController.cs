using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GEAR_SHOP.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
