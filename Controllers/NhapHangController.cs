using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels; // chứa NhapHangViewModel

namespace TL4_SHOP.Controllers
{
    [Authorize(Roles = "Admin, Nhân viên quản lý sản phẩm")]
    public class NhapHangController : Controller
    {
        private readonly _4tlShopContext _context;

        public NhapHangController(_4tlShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new NhapHangViewModel
            {
                DanhSachNhaCungCap = _context.NhaCungCaps.ToList(),
                DanhSachSanPham = _context.SanPhams.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ThemPhieuNhap(NhapHangViewModel model)
        {
            // Lấy NhanVienId từ session thay vì form
            var nhanVienId = HttpContext.Session.GetInt32("NhanVienId");

            if (nhanVienId == null)
            {
                return Content("Bạn chưa đăng nhập hoặc session hết hạn.");
            }

            var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.NhanVienId == nhanVienId.Value);
            if (nhanVien == null)
            {
                return Content("Không tìm thấy nhân viên.");
            }

            var sp = _context.SanPhams.Find(model.SanPhamId);
            if (sp == null)
            {
                return Content("Sản phẩm không tồn tại.");
            }

            var phieuNhap = new NhapHang
            {
                NhaCungCapId = model.NhaCungCapId,
                NgayNhap = DateTime.Now,
                NhanVienId = nhanVienId.Value
            };

            _context.NhapHangs.Add(phieuNhap);
            _context.SaveChanges();

            var chiTiet = new ChiTietNhapHang
            {
                PhieuNhapId = phieuNhap.PhieuNhapId,
                SanPhamId = model.SanPhamId,
                SoLuong = model.SoLuong,
                DonGiaNhap = model.DonGiaNhap,
                TongTien = model.SoLuong * model.DonGiaNhap
            };

            _context.ChiTietNhapHangs.Add(chiTiet);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
