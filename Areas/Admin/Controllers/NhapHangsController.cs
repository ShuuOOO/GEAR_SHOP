using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NhapHangsController : Controller
    {
        private readonly _4tlShopContext _context;

        public NhapHangsController(_4tlShopContext context) => _context = context;

        // GET: Admin/NhapHangs
        public async Task<IActionResult> Index(string? q, DateTime? from, DateTime? to, int page = 1, int pageSize = 20)
        {
            var query = _context.NhapHangs
                .AsNoTracking()
                .Include(x => x.NhaCungCap)
                .Include(x => x.ChiTietNhapHangs)
                .OrderByDescending(x => x.NgayNhap)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.NhaCungCap != null && x.NhaCungCap.TenNhaCungCap.Contains(q));
            if (from.HasValue) query = query.Where(x => x.NgayNhap >= from.Value);
            if (to.HasValue) query = query.Where(x => x.NgayNhap < to.Value.AddDays(1));

            var total = await query.CountAsync();
            var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Total = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Filter = new { q, from, to, page, pageSize };
            return View(data);
        }

        // GET: Admin/NhapHangs/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Suppliers = await _context.NhaCungCaps.AsNoTracking().OrderBy(x => x.TenNhaCungCap).ToListAsync();
            ViewBag.Products = await _context.SanPhams.AsNoTracking()
                                     .Select(p => new { p.SanPhamId, p.TenSanPham })
                                     .OrderBy(p => p.TenSanPham)
                                     .ToListAsync();
            return View(new NhapHangCreateViewModel());
        }

        // POST: Admin/NhapHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhapHangCreateViewModel vm)
        {
            // lọc lại các dòng hợp lệ
            var items = (vm.Items ?? new List<NhapHangItemViewModel>())
                .Where(i => i != null && i.SanPhamId.HasValue && i.SoLuong > 0)
                .ToList();

            if (!vm.NhaCungCapId.HasValue)
                ModelState.AddModelError(nameof(vm.NhaCungCapId), "Vui lòng chọn nhà cung cấp.");

            if (items.Count == 0)
                ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất 1 dòng sản phẩm hợp lệ.");

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(vm);
            }

            // GOM CHA–CON rồi SaveChanges 1 LẦN
            var phieu = new NhapHang
            {
                NhaCungCapId = vm.NhaCungCapId,
                NgayNhap = vm.NgayNhap ?? DateTime.Now,
                NhanVienId = vm.NhanVienId,
                ChiTietNhapHangs = items.Select(i => new ChiTietNhapHang
                {
                    SanPhamId = i.SanPhamId!.Value,
                    SoLuong = i.SoLuong,
                    DonGiaNhap = i.DonGiaNhap,               
                    TongTien = i.DonGiaNhap * i.SoLuong    
                }).ToList()
            };

            try
            {
                // cập nhật tồn kho cho từng sản phẩm
                foreach (var ct in phieu.ChiTietNhapHangs)
                {
                    var sp = await _context.SanPhams.FindAsync(ct.SanPhamId);
                    if (sp != null)
                    {
                        sp.SoLuongTon += ct.SoLuong;   // cộng thêm số lượng nhập
                    }
                }

                _context.NhapHangs.Add(phieu);

                await _context.SaveChangesAsync();   // EF tự bao trong 1 transaction nội bộ

                TempData["ok"] = "Tạo phiếu nhập thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Nếu trigger/FK báo lỗi → hiện ra cho dễ debug
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                await LoadDropdowns();
                return View(vm);
            }
        }
        private async Task LoadDropdowns()
        {
            ViewBag.Suppliers = await _context.NhaCungCaps
                .AsNoTracking()
                .OrderBy(x => x.TenNhaCungCap)
                .ToListAsync();

            ViewBag.Products = await _context.SanPhams
                .AsNoTracking()
                .Select(p => new { p.SanPhamId, p.TenSanPham })
                .OrderBy(p => p.TenSanPham)
                .ToListAsync();
        }

        // GET: Admin/NhapHangs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var entity = await _context.NhapHangs
                .Include(x => x.NhaCungCap)
                .Include(x => x.ChiTietNhapHangs).ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(x => x.PhieuNhapId == id);
            if (entity == null) return NotFound();
            return View(entity);
        }
    }
}
