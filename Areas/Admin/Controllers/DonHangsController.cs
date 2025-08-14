using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DonHangsController : Controller
    {
        private readonly _4tlShopContext _context;

        public DonHangsController(_4tlShopContext context)
        {
            _context = context;
        }

        // GET: Admin/DonHangs
        public async Task<IActionResult> Index(string? q, int? statusId, DateTime? from, DateTime? to, int page = 1, int pageSize = 12)
        {
            var query = _context.DonHangs
                .AsNoTracking()
                .Include(x => x.TrangThai)
                .Include(x => x.KhachHang)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.DonHangId.ToString().Contains(q) ||
                    (x.TenKhachHang != null && x.TenKhachHang.Contains(q)) ||
                    (x.EmailNguoiDat != null && x.EmailNguoiDat.Contains(q)));
            }
            if (statusId.HasValue)
            {
                query = query.Where(x => x.TrangThaiId == statusId);
            }
            if (from.HasValue)
            {
                query = query.Where(x => x.NgayDatHang >= from.Value);
            }
            if (to.HasValue)
            {
                var end = to.Value.Date.AddDays(1);
                query = query.Where(x => x.NgayDatHang < end);
            }

            int total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.NgayDatHang)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.DonHangId,
                    x.NgayDatHang,
                    x.TongTien,
                    TrangThai = x.TrangThai != null ? x.TrangThai.TenTrangThai : "",
                    // Ưu tiên TenKhachHang trên DonHang; fallback sang KhachHang.HoTen nếu có
                    TenKhachHang = !string.IsNullOrWhiteSpace(x.TenKhachHang)
                    ? x.TenKhachHang
                    : (x.KhachHang != null ? x.KhachHang.HoTen : ""),
                    x.EmailNguoiDat
                })

                .ToListAsync();

            ViewBag.Total = total;
            ViewBag.Filter = new { q, statusId, from, to, page, pageSize };
            ViewBag.Statuses = await _context.TrangThaiDonHangs.AsNoTracking().OrderBy(x => x.TrangThaiId).ToListAsync();

            return View(items);
        }

        // GET: Admin/DonHangs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var dh = await _context.DonHangs
                .AsNoTracking()
                .Include(x => x.TrangThai)
                .Include(x => x.ChiTietDonHangs).ThenInclude(ct => ct.SanPham)
                .Include(x => x.DiaChi)
                .FirstOrDefaultAsync(x => x.DonHangId == id);

            if (dh == null) return NotFound();

            var vm = new DonHangDetailViewModel
            {
                DonHangId = dh.DonHangId,

                // Ưu tiên TenKhachHang lưu trực tiếp trên DonHang; fallback sang KhachHang.HoTen
                TenKhachHang = !string.IsNullOrWhiteSpace(dh.TenKhachHang)
        ? dh.TenKhachHang
        : (dh.KhachHang != null ? dh.KhachHang.HoTen : string.Empty),

                // Ưu tiên số ĐT trên DonHang; fallback sang DiaChi.Phone
                SoDienThoai = !string.IsNullOrWhiteSpace(dh.SoDienThoai)
                ? dh.SoDienThoai
                : (dh.DiaChi != null ? dh.DiaChi.Phone : string.Empty),

                // Nếu DonHang đã có DiaChiGiaoHang thì dùng luôn; nếu không thì ghép từ DiaChi
                DiaChiGiaoHang = !string.IsNullOrWhiteSpace(dh.DiaChiGiaoHang)
        ? dh.DiaChiGiaoHang
        : (dh.DiaChi != null
            ? string.Join(", ",
                new[] { dh.DiaChi.SoNha, dh.DiaChi.PhuongXa, dh.DiaChi.QuanHuyen, dh.DiaChi.ThanhPho }
                .Where(s => !string.IsNullOrWhiteSpace(s)))
            : string.Empty),

                NgayDatHang = dh.NgayDatHang,
                TongTien = dh.TongTien,

                // EF đặt tên chuẩn PascalCase: TrangThaiId
                TrangThai = dh.TrangThaiId,

                ChiTiet = dh.ChiTietDonHangs.Select(i => new ChiTietDonHangViewModel
                {
                    SanPhamId = i.SanPhamId,
                    TenSanPham = i.SanPham != null ? i.SanPham.TenSanPham : "",
                    SoLuong = i.SoLuong,
                    DonGia = i.DonGia,
                    ThanhTien = i.ThanhTien
                }).ToList()
            };

            ViewBag.Statuses = await _context.TrangThaiDonHangs.AsNoTracking().OrderBy(x => x.TrangThaiId).ToListAsync();
            return View(vm);
        }

        // POST: Admin/DonHangs/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int statusId)
        {
            var entity = await _context.DonHangs.FindAsync(id);
            if (entity == null) return NotFound();

            bool exists = await _context.TrangThaiDonHangs.AsNoTracking().AnyAsync(x => x.TrangThaiId == statusId);
            if (!exists)
            {
                TempData["Error"] = "Trạng thái không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id });
            }

            entity.TrangThaiId = statusId;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật trạng thái đơn hàng.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Admin/DonHangs/Invoice/5
        public async Task<IActionResult> Invoice(int id)
        {
            var dh = await _context.DonHangs
                .Include(x => x.ChiTietDonHangs).ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(x => x.DonHangId == id);
            if (dh == null) return NotFound();

            var bytes = InvoiceGenerator.CreateInvoice(dh);
            return File(bytes, "application/pdf", $"invoice-{id}.pdf");
        }
    }
}