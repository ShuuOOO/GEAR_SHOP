using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TL4_SHOP.Controllers
{
    public class ChamSocKhachHangController : Controller
    {
        [Authorize(Roles = "Nhân viên chăm sóc khách hàng, Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
