using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NhaCungCapsController : Controller
    {
        private readonly _4tlShopContext _context;
        public NhaCungCapsController(_4tlShopContext context) => _context = context;

        // GET: Admin/NhaCungCaps
        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.NhaCungCaps.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.TenNhaCungCap.Contains(q) || x.Email.Contains(q) || x.Phone.Contains(q));
            var data = await query.OrderBy(x => x.TenNhaCungCap).ToListAsync();
            return View(data);
        }

        public IActionResult Create() => View(new NhaCungCap());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaCungCap model)
        {
            if (!ModelState.IsValid) return View(model);
            // Tự sinh khoá vì bảng không dùng IDENTITY
            if (model.NhaCungCapId == 0)
            {
                var next = await _context.NhaCungCaps.MaxAsync(x => (int?)x.NhaCungCapId) ?? 0;
                model.NhaCungCapId = next + 1;
            }
            _context.NhaCungCaps.Add(model);
            await _context.SaveChangesAsync();
            TempData["ok"] = "Thêm nhà cung cấp thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.NhaCungCaps.FindAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhaCungCap model)
        {
            if (id != model.NhaCungCapId) return BadRequest();
            if (!ModelState.IsValid) return View(model);
            _context.NhaCungCaps.Update(model);
            await _context.SaveChangesAsync();
            TempData["ok"] = "Cập nhật nhà cung cấp thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.NhaCungCaps.FindAsync(id);
            if (entity != null)
            {
                _context.NhaCungCaps.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["ok"] = "Đã xoá nhà cung cấp.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
