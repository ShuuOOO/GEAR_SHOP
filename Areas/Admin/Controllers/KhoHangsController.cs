using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class KhoHangsController : Controller
    {
        private readonly _4tlShopContext _context;
        public KhoHangsController(_4tlShopContext context) => _context = context;

        // GET: Admin/KhoHangs
        public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 25)
        {
            var query = _context.KhoHangs.Include(k => k.SanPham).AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.SanPham.TenSanPham.Contains(q));

            var total = await query.CountAsync();
            var data = await query.OrderBy(x => x.SanPham.TenSanPham)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new { x.SanPhamId, TenSanPham = x.SanPham.TenSanPham, SoLuongTon = x.SoLuongTon })
                .ToListAsync();

            ViewBag.Total = total; ViewBag.Page = page; ViewBag.PageSize = pageSize; ViewBag.Filter = new { q, page, pageSize };
            return View(data);
        }

        // GET: Admin/KhoHangs/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.KhoHangs.Include(x => x.SanPham).FirstOrDefaultAsync(x => x.SanPhamId == id);
            if (entity == null)
            {
                var sp = await _context.SanPhams.FindAsync(id);
                if (sp == null) return NotFound();
                entity = new KhoHang { SanPhamId = id, SanPham = sp, SoLuongTon = 0 };
            }
            return View(entity);
        }

        // POST: Admin/KhoHangs/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int sanPhamId, int soLuongTon)
        {
            if (soLuongTon < 0)
            {
                ModelState.AddModelError(string.Empty, "Số lượng tồn không được âm.");
                var entity = await _context.KhoHangs.Include(x => x.SanPham)
                                 .FirstOrDefaultAsync(x => x.SanPhamId == sanPhamId);
                if (entity == null)
                {
                    var sp = await _context.SanPhams.FindAsync(sanPhamId);
                    entity = new KhoHang { SanPhamId = sanPhamId, SanPham = sp, SoLuongTon = 0 };
                }
                return View(entity);
            }

            var kh = await _context.KhoHangs.FirstOrDefaultAsync(x => x.SanPhamId == sanPhamId);
            if (kh == null) _context.KhoHangs.Add(new KhoHang { SanPhamId = sanPhamId, SoLuongTon = soLuongTon });
            else { kh.SoLuongTon = soLuongTon; _context.KhoHangs.Update(kh); }

            await _context.SaveChangesAsync();
            TempData["ok"] = "Cập nhật tồn kho thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
