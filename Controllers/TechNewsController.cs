using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

namespace TL4_SHOP.Controllers
{
    public class TechNewsController : BaseController
    {
        public TechNewsController(_4tlShopContext context) : base(context) { }

        // /TechNews?page=1&search=
        public async Task<IActionResult> Index(int page = 1, string? search = null)
        {
            const int pageSize = 8;
            var query = _context.TechNews.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = search.Trim();
                query = query.Where(n =>
                    n.Title.Contains(kw) || (n.Summary != null && n.Summary.Contains(kw)) || (n.Tags != null && n.Tags.Contains(kw)));
            }

            query = query.OrderByDescending(n => n.PublishedAt);
            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;
            ViewBag.Search = search;

            return View(items);
        }

        // /TechNews/Detail/{slug}
        public async Task<IActionResult> Detail(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();

            var item = await _context.TechNews.FirstOrDefaultAsync(n => n.Slug == slug);
            if (item == null) return NotFound();

            // tăng view
            item.ViewCount += 1;
            await _context.SaveChangesAsync();

            return View(item);
        }
    }
}
