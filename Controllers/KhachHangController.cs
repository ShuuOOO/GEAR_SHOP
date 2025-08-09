using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Controllers
{
    [Authorize(Roles = "Nhân viên quản lý nhân sự, Admin")]
    public class KhachHangController : Controller
    {
        private readonly _4tlShopContext _context;

        public KhachHangController(_4tlShopContext context)
        {
            _context = context;
        }

        public IActionResult KhachHang()
        {
            // Lấy danh sách khách hàng kèm theo danh sách địa chỉ
            var khachHangs = _context.KhachHangs
                .Include(k => k.DiaChis) // Load địa chỉ của khách
                .ToList();

            return View(khachHangs);
        }
    }
}
