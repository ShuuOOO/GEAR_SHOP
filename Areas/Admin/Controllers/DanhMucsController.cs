using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DanhMucsController : Controller
    {
        private readonly _4tlShopContext _context;

        public DanhMucsController(_4tlShopContext context)
        {
            _context = context;
        }

        // GET: Admin/DanhMucs
        public async Task<IActionResult> Index(string? q, int? parentId, int page = 1, int pageSize = 12)
        {
            var query = _context.DanhMucSanPhams
                .AsNoTracking()
                .Include(x => x.DanhMucCha)
                .Include(x => x.SanPhams)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.TenDanhMuc.Contains(q));
            }
            if (parentId.HasValue)
            {
                query = query.Where(x => x.DanhMucChaId == parentId);
            }

            int total = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.TenDanhMuc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DanhMucViewModel
                {
                    Id = x.DanhMucId,
                    TenDanhMuc = x.TenDanhMuc,
                    DanhMucChaId = x.DanhMucChaId,
                    SoLuongSanPham = x.SanPhams.Count
                })
                .ToListAsync();

            ViewBag.Total = total;
            ViewBag.Filter = new { q, parentId, page, pageSize };
            ViewBag.Parents = await _context.DanhMucSanPhams.AsNoTracking().OrderBy(x => x.TenDanhMuc).ToListAsync();

            return View(items);
        }

        // GET: Admin/DanhMucs/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Parents = await _context.DanhMucSanPhams.AsNoTracking().OrderBy(x => x.TenDanhMuc).ToListAsync();
            return View();
        }

        // POST: Admin/DanhMucs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDanhMuc,MoTa,DanhMucChaId")] DanhMucSanPham input)
        {
            // [THÊM] bỏ yêu cầu validate cho DanhMucId để EF tự sinh
            ModelState.Remove(nameof(DanhMucSanPham.DanhMucId));

            // [THÊM] chuẩn hoá dữ liệu & validate trùng tên trong cùng cấp
            if (!string.IsNullOrWhiteSpace(input.TenDanhMuc))
                input.TenDanhMuc = input.TenDanhMuc.Trim();

            var isDup = await _context.DanhMucSanPhams
                .AsNoTracking()
                .AnyAsync(x => x.TenDanhMuc == input.TenDanhMuc && x.DanhMucChaId == input.DanhMucChaId);

            if (isDup)
                ModelState.AddModelError(nameof(input.TenDanhMuc), "Danh mục đã tồn tại trong cùng cấp.");

            if (!ModelState.IsValid)
            {
                ViewBag.Parents = await _context.DanhMucSanPhams.AsNoTracking().OrderBy(x => x.TenDanhMuc).ToListAsync();
                return View(input);
            }

            _context.DanhMucSanPhams.Add(input);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã tạo danh mục.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/DanhMucs/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.DanhMucSanPhams.FindAsync(id);
            if (entity == null) return NotFound();
            ViewBag.Parents = await _context.DanhMucSanPhams
                .AsNoTracking()
                .Where(x => x.DanhMucId != id) // không cho chọn chính nó
                .OrderBy(x => x.TenDanhMuc)
                .ToListAsync();
            return View(entity);
        }

        // POST: Admin/DanhMucs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DanhMucId,TenDanhMuc,MoTa,DanhMucChaId")] DanhMucSanPham input)
        {
            if (id != input.DanhMucId) return BadRequest();

            // [THÊM] chuẩn hoá
            if (!string.IsNullOrWhiteSpace(input.TenDanhMuc))
                input.TenDanhMuc = input.TenDanhMuc.Trim();

            // [THÊM] không cho chọn chính nó làm cha
            if (input.DanhMucChaId == id)
                ModelState.AddModelError(nameof(input.DanhMucChaId), "Không thể chọn chính danh mục này làm danh mục cha.");

            // [THÊM] không cho tạo chu kỳ cha-con (newParent không phải là hậu duệ của id)
            if (input.DanhMucChaId.HasValue)
            {
                var parentId = input.DanhMucChaId;
                while (parentId.HasValue)
                {
                    if (parentId.Value == id)
                    {
                        ModelState.AddModelError(nameof(input.DanhMucChaId), "Danh mục cha không hợp lệ (tạo chu kỳ).");
                        break;
                    }
                    parentId = await _context.DanhMucSanPhams
                        .Where(x => x.DanhMucId == parentId.Value)
                        .Select(x => x.DanhMucChaId)
                        .FirstOrDefaultAsync();
                }
            }

            // [THÊM] check trùng tên trong cùng cấp (trừ chính nó)
            var isDup = await _context.DanhMucSanPhams
                .AsNoTracking()
                .AnyAsync(x => x.DanhMucId != id
                               && x.TenDanhMuc == input.TenDanhMuc
                               && x.DanhMucChaId == input.DanhMucChaId);
            if (isDup)
                ModelState.AddModelError(nameof(input.TenDanhMuc), "Danh mục đã tồn tại trong cùng cấp.");

            if (!ModelState.IsValid)
            {
                ViewBag.Parents = await _context.DanhMucSanPhams
                    .AsNoTracking()
                    .Where(x => x.DanhMucId != id)
                    .OrderBy(x => x.TenDanhMuc).ToListAsync();
                return View(input);
            }

            var entity = await _context.DanhMucSanPhams.FindAsync(id);
            if (entity == null) return NotFound();

            entity.TenDanhMuc = input.TenDanhMuc;
            entity.MoTa = input.MoTa?.Trim() ?? string.Empty; // [SỬA] chuẩn hoá chuỗi
            entity.DanhMucChaId = input.DanhMucChaId;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật danh mục.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/DanhMucs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.DanhMucSanPhams
                .Include(x => x.SanPhams)
                .Include(x => x.DanhMucCon)
                .FirstOrDefaultAsync(x => x.DanhMucId == id);
            if (entity == null) return NotFound();

            if (entity.SanPhams.Any() || entity.DanhMucCon.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục đang có sản phẩm hoặc danh mục con.";
                return RedirectToAction(nameof(Index));
            }

            _context.DanhMucSanPhams.Remove(entity);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa danh mục.";
            return RedirectToAction(nameof(Index));
        }
    }
}