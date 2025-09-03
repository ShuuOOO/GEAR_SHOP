using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrCustomerCare")]
    public class ChamSocKhachHangsController : Controller
    {
        private readonly _4tlShopContext _context;

        public ChamSocKhachHangsController(_4tlShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new ChamSocKhachHangViewModel
            {
                Users = _context.TaoTaiKhoans
                    .Where(u => u.VaiTro == "Khách hàng")
                    .ToList()
            };
            return View(model);
        }
    }
}
