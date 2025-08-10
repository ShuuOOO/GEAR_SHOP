using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên quản lý đơn hàng")]
    public class DonHangController : Controller
    {
        private readonly _4tlShopContext _context;

        public DonHangController(_4tlShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var donHangs = _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.TrangThai)
                .Include(d => d.DiaChi)
                .ToList();

            return View(donHangs);
        }
    }
}
