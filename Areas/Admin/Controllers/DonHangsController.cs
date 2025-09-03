using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;
using Microsoft.Data.SqlClient;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrOrderManager")]
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
                .Include(x => x.KhachHang)
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

            // 1) Validate trạng thái đích có tồn tại
            bool exists = await _context.TrangThaiDonHangs.AsNoTracking()
                .AnyAsync(x => x.TrangThaiId == statusId);
            if (!exists)
            {
                TempData["Error"] = "Trạng thái không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // 2) Không làm gì nếu không thay đổi
            if (entity.TrangThaiId == statusId)
            {
                TempData["Info"] = "Trạng thái không thay đổi.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // 3) Ràng buộc luồng chuyển trạng thái cơ bản (chỉ tiến/hoặc hủy)
            //    1: Chờ xác nhận, 2: Đã xác nhận, 3: Đang giao, 4: Giao thành công, 5: Đã hủy
            // 3) Ràng buộc luồng chuyển trạng thái cơ bản (chỉ tiến/hoặc hủy)
            var current = entity.TrangThaiId;
            var next = statusId;

            if (current < 1 || current > 5)
            {
                TempData["Error"] = $"Trạng thái hiện tại ({current}) không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id });
            }
            if (next < 1 || next > 5)
            {
                TempData["Error"] = $"Trạng thái đích ({next}) không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Chặn nếu đơn đã ở trạng thái kết thúc vòng đời
            if (IsTerminal(current))
            {
                TempData["Warning"] = "Đơn hàng đã kết thúc (giao xong/đã hủy), không thể đổi trạng thái.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Chỉ cho phép chuyển theo AllowedTransitions
            if (!IsTransitionAllowed(current, next))
            {
                TempData["Error"] = $"Không cho phép chuyển từ trạng thái {current} sang {next}.";
                return RedirectToAction(nameof(Details), new { id });
            }
            // Nếu chuyển 2→3 (đang giao) hoặc 3→4 (giao xong), bắt buộc đơn phải có chi tiết > 0
            if ((current == 2 && next == 3) || (current == 3 && next == 4))
            {
                // Đếm nhanh số dòng chi tiết > 0
                var hasItems = await _context.ChiTietDonHangs
                    .AsNoTracking()
                    .AnyAsync(ct => ct.DonHangId == id && ct.SoLuong > 0);

                if (!hasItems)
                {
                    TempData["Error"] = "Đơn hàng chưa có sản phẩm hợp lệ (số lượng > 0). Không thể chuyển trạng thái.";
                    return RedirectToAction(nameof(Details), new { id });
                }
            }

            // 4) Cập nhật
            entity.TrangThaiId = statusId;
            // Ghi nhận cập nhật để audit (không đổi tên field public nào khác)

            bool isAjax = string.Equals(Request?.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

            try
            {
                // Trigger DB sẽ tự trừ/hoàn tồn khi vào/ra trạng thái 'Giao thành công'
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã cập nhật trạng thái đơn hàng.";

                if (isAjax)
                    return Json(new { ok = true, message = TempData["Success"], status = statusId });

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "Có xung đột dữ liệu. Vui lòng tải lại trang và thử lại.";
                if (isAjax) return Json(new { ok = false, message = TempData["Error"] });
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                // Lỗi từ trigger/ràng buộc kho (âm tồn, v.v.)
                // Nếu bạn có quy ước Number thì có thể map thân thiện hơn theo sqlEx.Number
                TempData["Error"] = "Cập nhật thất bại do ràng buộc kho: " + sqlEx.Message;
                if (isAjax) return Json(new { ok = false, message = TempData["Error"] });
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi không xác định: " + ex.Message;
                if (isAjax) return Json(new { ok = false, message = TempData["Error"] });
                return RedirectToAction(nameof(Details), new { id });
            }
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

        private static readonly Dictionary<int, int[]> AllowedTransitions = new()
{
            // 1: Mới tạo -> 2: Xác nhận | 5: Hủy
            { 1, new[] { 2, 5 } },
            // 2: Xác nhận -> 3: Đang giao | 5: Hủy
            { 2, new[] { 3, 5 } },
            // 3: Đang giao -> 4: Giao thành công | 5: Hủy
            { 3, new[] { 4, 5 } },
            // 4,5: Kết thúc vòng đời -> không cho đổi nữa
        };
        private static bool IsTransitionAllowed(int current, int next)
        {
            if (!AllowedTransitions.TryGetValue(current, out var nexts) || nexts == null)
                return false;
            return Array.IndexOf(nexts, next) >= 0;
        }

        private static bool IsTerminal(int statusId) => statusId == 4 || statusId == 5;
    }
}