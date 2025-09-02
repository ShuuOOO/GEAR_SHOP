using Microsoft.AspNetCore.Mvc;
using TL4_SHOP.Data;
using TL4_SHOP.Extensions;
using TL4_SHOP.Models;
using TL4_SHOP.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using TL4_SHOP.Hubs;

namespace TL4_SHOP.Controllers
{
    public class OrderController : BaseController
    {
        private readonly _4tlShopContext _context;
        private readonly IHubContext<NotificationHub> _notifyHub;

        public OrderController(_4tlShopContext context, IHubContext<NotificationHub> notifyHub) : base(context)
        {
            _context = context;          // <<< QUAN TRỌNG: gán field để dùng ở các action
            _notifyHub = notifyHub;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("GioHang");
            if (cart == null || cart.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng trống!";
                return RedirectToAction("ShoppingCart", "Home");
            }

            // Sinh mã đơn tại đây (dùng được cho QR, form, email, PDF…)
            string maDonHang = "DH_" + new Random().Next(100000, 999999);
            ViewBag.MaDonHang = maDonHang;

            return View();
        }


        [HttpPost]
        public IActionResult Checkout(string tenKhachHang, string soDienThoai, string diaChiGiaoHang, string ghiChu, string phuongThucThanhToan, int phiVanChuyen, decimal tongTien)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("GioHang");
            if (cart == null || cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Checkout");
            }

            int? taiKhoanId = null;
            string email = "";

            if (User.Identity.IsAuthenticated)
            {
                email = User.FindFirstValue(ClaimTypes.Email);
                var taiKhoan = _context.TaoTaiKhoans.FirstOrDefault(x => x.Email == email);
                if (taiKhoan != null)
                {
                    taiKhoanId= taiKhoan.TaiKhoanId;
                }
            }
            else
            {
                email = Request.Form["emailKhach"]; // thêm input trong form cho khách vãng lai
            }

            var donHang = new DonHang
            {
                TenKhachHang = tenKhachHang,
                SoDienThoai = soDienThoai,
                DiaChiGiaoHang = diaChiGiaoHang,
                GhiChu = ghiChu,
                PhuongThucThanhToan = phuongThucThanhToan,
                NgayDatHang = DateTime.Now,
                PhiVanChuyen = phiVanChuyen,
                TrangThaiId = 1,
                TrangThaiDonHangText = "Chờ xác nhận",
                TongTien = tongTien,
                TaiKhoanId = taiKhoanId,
                EmailNguoiDat = email  // ➕ thêm cột này vào bảng DonHang
            };

            foreach (var item in cart)
            {
                donHang.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    SanPhamId = item.SanPhamId,
                    SoLuong = item.SoLuong,
                    DonGia = item.Gia,
                    ThanhTien = item.Gia * item.SoLuong
                });
            }

            _context.DonHangs.Add(donHang);
            _context.SaveChanges();

            // Gửi realtime tới Admin (giữ action sync, nên dùng GetAwaiter().GetResult())
            _notifyHub.Clients.All.SendAsync("NewOrder", new
            {
                id = donHang.DonHangId,
                customer = tenKhachHang,
                total = (decimal)tongTien + phiVanChuyen,
                createdAt = donHang.NgayDatHang.ToString("HH:mm dd/MM")
            }).GetAwaiter().GetResult();

            // Gán mã đơn hàng cho QR 
            TempData["MaDonHang"] = $"DH_{donHang.DonHangId}";


            // Xoá giỏ hàng
            HttpContext.Session.Remove("GioHang");

            // Gửi email xác nhận nếu có
            SendConfirmationEmail(email, donHang);

            return RedirectToAction("Detail", new { id = donHang.DonHangId });
        }

        private void SendConfirmationEmail(string email, DonHang donHang)
        {
            // Chỉ để tránh warning "biến không dùng"
            var _ = email;
            var __ = donHang;

            // Bạn có thể log ra console hoặc bỏ trống hoàn toàn
            Console.WriteLine($"[DEBUG] Gọi SendConfirmationEmail cho đơn hàng #{donHang?.DonHangId}");
        }


        [HttpGet]
        public IActionResult Track(int id, string email)
        {
            var donHang = _context.DonHangs
                .Where(d => d.DonHangId == id && d.EmailNguoiDat == email)
                .Select(d => new DonHangDetailViewModel
                {
                    DonHangId = d.DonHangId,
                    TenKhachHang = d.TenKhachHang,
                    DiaChiGiaoHang = d.DiaChiGiaoHang,
                    SoDienThoai = d.SoDienThoai,
                    NgayDatHang = d.NgayDatHang,
                    TongTien = d.TongTien,
                    TrangThai = d.TrangThai != null ? d.TrangThai.TrangThaiId : 0,
                    ChiTiet = d.ChiTietDonHangs.Select(c => new ChiTietDonHangViewModel
                    {
                        SanPhamId = c.SanPhamId,
                        TenSanPham = c.SanPham.TenSanPham,
                        SoLuong = c.SoLuong,
                        DonGia = c.DonGia,
                        ThanhTien = c.ThanhTien
                    }).ToList()
                })
                .FirstOrDefault();

            if (donHang == null)
            {
                ViewBag.Message = "Không tìm thấy đơn hàng hoặc email không khớp.";
                return View("TrackResult", null);
            }

            return View("TrackResult", donHang);
        }
        public IActionResult Complete()
        {
            return View(); // Trang cảm ơn
        }

        public IActionResult Detail(int id)
        {
            var donHang = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .Where(d => d.DonHangId == id)
                .Select(d => new DonHangDetailViewModel
                {
                    DonHangId = d.DonHangId,
                    TenKhachHang = d.TenKhachHang,
                    SoDienThoai = d.SoDienThoai,
                    DiaChiGiaoHang = d.DiaChiGiaoHang,
                    NgayDatHang = d.NgayDatHang,
                    TongTien = d.TongTien,
                    TrangThai = d.TrangThai != null ? d.TrangThai.TrangThaiId : 0,
                    ChiTiet = d.ChiTietDonHangs.Select(c => new ChiTietDonHangViewModel
                    {
                        SanPhamId = c.SanPhamId,
                        TenSanPham = c.SanPham.TenSanPham,
                        SoLuong = c.SoLuong,
                        DonGia = c.DonGia,
                        ThanhTien = c.ThanhTien
                    }).ToList()
                })
                .FirstOrDefault();

            if (donHang == null) return NotFound();

            // Load lại đơn đầy đủ (có SanPham) để gửi PDF
            var donHangFull = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefault(d => d.DonHangId == id);

            if (!string.IsNullOrEmpty(donHangFull?.EmailNguoiDat))
            {
                SendConfirmationEmail(donHangFull.EmailNguoiDat, donHangFull);
            }

            return View(donHang);
        }



        [Authorize]
        public IActionResult LichSu()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var taiKhoan = _context.TaoTaiKhoans
            .FirstOrDefault(x => x.Email == email
                  && (x.LoaiTaiKhoan == "KhachHang" || x.LoaiTaiKhoan == "NhanVien"));


            if (taiKhoan == null || taiKhoan.TaiKhoanId == null)
                return RedirectToAction("Login", "Account");

            var donHangs = _context.DonHangs
                .Where(d => d.TaiKhoanId == taiKhoan.TaiKhoanId)
                .OrderByDescending(d => d.NgayDatHang)
                .Select(d => new DonHangDetailViewModel
                {
                    DonHangId = d.DonHangId,
                    NgayDatHang = d.NgayDatHang,
                    TongTien = d.TongTien,
                    TrangThai = d.TrangThai != null ? d.TrangThai.TrangThaiId : 0,
                })
                .ToList();

            return View("LichSu", donHangs);
        }
        [HttpGet]
        public IActionResult TaiHoaDon(int id)
        {
            var donHang = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefault(d => d.DonHangId == id);

            if (donHang == null)
                return NotFound("Không tìm thấy đơn hàng.");

            var pdfBytes = InvoiceGenerator.CreateInvoice(donHang);

            return File(pdfBytes, "application/pdf", $"HoaDon_{donHang.DonHangId}.pdf");
        }

        public IActionResult Cancel(int id)
        {
            var donHang = _context.DonHangs.FirstOrDefault(d => d.DonHangId == id);

            if (donHang == null)
            {
                return NotFound();
            }

            // Nếu đã giao hoặc đã hủy thì không cho hủy nữa
            if (donHang.TrangThaiId == 4 || donHang.TrangThaiId == 5)
            {
                TempData["Message"] = "Đơn hàng không thể hủy.";
                return RedirectToAction("Detail", new { id = id });
            }

            // Cập nhật trạng thái đơn hàng thành ĐÃ HỦY
            donHang.TrangThaiId = 5; // ID = 5 là "Đã hủy"
            donHang.TrangThaiDonHangText = "Đã hủy";
            _context.SaveChanges();

            TempData["Message"] = "Đơn hàng đã được hủy.";
            return RedirectToAction("Detail", new { id = id });
        }
    }
}
