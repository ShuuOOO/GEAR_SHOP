using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TechNewsController : Controller
    {
        private readonly _4tlShopContext _context;
        private readonly IWebHostEnvironment _env;
        public TechNewsController(_4tlShopContext context, IWebHostEnvironment env)
        {
            _context = context; _env = env;
        }

        // GET: Admin/TechNews
        public async Task<IActionResult> Index(string? q, bool? featured, DateTime? from, DateTime? to,
                                               int page = 1, int pageSize = 12)
        {
            var query = _context.TechNews.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.Title.Contains(q) || x.Slug.Contains(q) || (x.Tags != null && x.Tags.Contains(q)));
            }
            if (featured.HasValue) query = query.Where(x => x.IsFeatured == featured.Value);
            if (from.HasValue) query = query.Where(x => x.PublishedAt >= from.Value);
            if (to.HasValue) query = query.Where(x => x.PublishedAt <= to.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Total = total;
            ViewBag.Filter = new { q, featured, from, to, page, pageSize };
            return View(items);
        }

        // GET: Admin/TechNews/Create
        public IActionResult Create()
        {
            return View(new TechNews
            {
                PublishedAt = DateTime.UtcNow,
                IsFeatured = false
            });
        }

        // POST: Admin/TechNews/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TechNews model, IFormFile? CoverImageFile)
        {
            // Tạo slug nếu trống
            if (string.IsNullOrWhiteSpace(model.Slug)) model.Slug = Slugify(model.Title);

            // Unique slug
            if (await _context.TechNews.AnyAsync(x => x.Slug == model.Slug))
                ModelState.AddModelError(nameof(model.Slug), "Slug đã tồn tại, hãy đổi một giá trị khác.");

            if (!ModelState.IsValid) return View(model);

            if (CoverImageFile != null && CoverImageFile.Length > 0)
                model.CoverImage = await SaveImageAsync(CoverImageFile);

            // Đảm bảo UTC cho PublishedAt nếu bạn đang lưu UTC trong DB
            model.PublishedAt = DateTime.SpecifyKind(model.PublishedAt, DateTimeKind.Utc);

            _context.TechNews.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã tạo bài viết thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/TechNews/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var n = await _context.TechNews.FindAsync(id);
            if (n == null) return NotFound();
            return View(n);
        }

        // POST: Admin/TechNews/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TechNews model, IFormFile? CoverImageFile)
        {
            if (id != model.TechNewsId) return NotFound();

            if (string.IsNullOrWhiteSpace(model.Slug)) model.Slug = Slugify(model.Title);
            if (await _context.TechNews.AnyAsync(x => x.TechNewsId != id && x.Slug == model.Slug))
                ModelState.AddModelError(nameof(model.Slug), "Slug đã tồn tại, hãy đổi một giá trị khác.");
            if (!ModelState.IsValid) return View(model);

            var n = await _context.TechNews.FirstOrDefaultAsync(x => x.TechNewsId == id);
            if (n == null) return NotFound();

            n.Title = model.Title;
            n.Slug = model.Slug;
            n.Summary = model.Summary;
            n.ContentHtml = model.ContentHtml;
            n.Author = model.Author;
            n.Tags = model.Tags;
            n.IsFeatured = model.IsFeatured;
            n.PublishedAt = DateTime.SpecifyKind(model.PublishedAt, DateTimeKind.Utc);

            if (CoverImageFile != null && CoverImageFile.Length > 0)
                n.CoverImage = await SaveImageAsync(CoverImageFile);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật bài viết.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/TechNews/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var n = await _context.TechNews.FindAsync(id);
            if (n == null)
            {
                TempData["Error"] = "Bài viết không tồn tại hoặc đã bị xoá.";
                return RedirectToAction(nameof(Index));
            }

            _context.TechNews.Remove(n);
            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã xoá bài viết “{n.Title}”.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = "Xoá thất bại. " + (ex.InnerException?.Message ?? ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        // Helpers
        async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath ?? "", "images", "news");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folder, fileName);
            using (var stream = System.IO.File.Create(fullPath))
                await file.CopyToAsync(stream);
            return Path.Combine("images", "news", fileName).Replace("\\", "/");
        }

        static string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            string s = input.ToLowerInvariant();
            s = s.Normalize(System.Text.NormalizationForm.FormD);
            var chars = s.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark);
            s = new string(chars.ToArray());
            s = System.Text.RegularExpressions.Regex.Replace(s, @"[^a-z0-9\s-]", "");
            s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", "-").Trim('-');
            s = System.Text.RegularExpressions.Regex.Replace(s, "-{2,}", "-");
            return s;
        }
    }
}
