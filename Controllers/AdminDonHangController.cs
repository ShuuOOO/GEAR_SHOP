using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên quản lý đơn hàng")]
    public class AdminDonHangController : Controller
    {
        private readonly _4tlShopContext _context;

        public AdminDonHangController(_4tlShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var donHangs = _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.TrangThai)
                .ToList();

            ViewBag.TrangThais = _context.TrangThaiDonHangs.ToList();
            return View(donHangs);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, int newStatus)
        {
            var don = _context.DonHangs.Find(id);
            if (don != null)
            {
                don.TrangThaiId = newStatus;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
