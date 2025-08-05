using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace TL4_SHOP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSanPhamController : Controller
    {
        private readonly _4tlShopContext _context;

        public AdminSanPhamController(_4tlShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int? categoryId, string stockStatus)
        {
            // Lấy danh sách sản phẩm
            var sanPhams = from s in _context.SanPhams.Include(s => s.DanhMuc).Include(s => s.NhaCungCap)
                           select s;

            // Áp dụng các bộ lọc
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sanPhams = sanPhams.Where(s => s.TenSanPham.Contains(searchTerm));
                ViewBag.SearchTerm = searchTerm;
            }

            if (categoryId.HasValue)
            {
                sanPhams = sanPhams.Where(s => s.DanhMucId == categoryId);
                ViewBag.CategoryId = categoryId;
            }

            if (!string.IsNullOrEmpty(stockStatus))
            {
                switch (stockStatus)
                {
                    case "instock":
                        sanPhams = sanPhams.Where(s => s.SoLuongTon > 5);
                        break;
                    case "lowstock":
                        sanPhams = sanPhams.Where(s => s.SoLuongTon <= 5 && s.SoLuongTon > 0);
                        break;
                    case "outstock":
                        sanPhams = sanPhams.Where(s => s.SoLuongTon == 0);
                        break;
                }
                ViewBag.StockStatus = stockStatus;
            }

            // Tạo dropdown cho danh mục
            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc", categoryId);

            // Tạo dropdown cho trạng thái kho
            var stockStatusList = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Tất cả" },
            new SelectListItem { Value = "instock", Text = "Còn hàng" },
            new SelectListItem { Value = "lowstock", Text = "Sắp hết" },
            new SelectListItem { Value = "outstock", Text = "Hết hàng" }
        };
            ViewBag.StockStatusList = new SelectList(stockStatusList, "Value", "Text", stockStatus);

            return View(sanPhams.AsEnumerable());
        }


        // GET: AdminSanPham/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.NhaCungCap)
                .FirstOrDefaultAsync(m => m.SanPhamId == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: AdminSanPham/Create
        public IActionResult Create()
        {
            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc");
            ViewBag.NhaCungCaps = new SelectList(_context.NhaCungCaps, "NhaCungCapId", "TenNhaCungCap");
            return View();
        }

        // POST: AdminSanPham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TL4_SHOP.Models.SanPham sanPham)
        {
            if (sanPham.HinhAnhFile != null)
            {
                // Tạo tên file ngẫu nhiên + đuôi ảnh
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(sanPham.HinhAnhFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await sanPham.HinhAnhFile.CopyToAsync(stream);
                }

                sanPham.HinhAnh = fileName; // Gán tên file cho thuộc tính HinhAnh
            }

            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Load lại dropdown khi lỗi
            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc");
            ViewBag.NhaCungCaps = new SelectList(_context.NhaCungCaps, "NhaCungCapId", "TenNhaCungCap");
            return View(sanPham);
        }


        // GET: AdminSanPham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc");
            ViewBag.NhaCungCaps = new SelectList(_context.NhaCungCaps, "NhaCungCapId", "TenNhaCungCap");
            return View(sanPham);
        }

        // POST: AdminSanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TL4_SHOP.Models.SanPham sanPham)
        {
            if (id != sanPham.SanPhamId)
                return NotFound();

            // Lấy entity hiện tại từ database
            var existingSanPham = await _context.SanPhams.FindAsync(id);
            if (existingSanPham == null)
                return NotFound();

            // Xử lý upload ảnh nếu có
            if (sanPham.HinhAnhFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(sanPham.HinhAnhFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await sanPham.HinhAnhFile.CopyToAsync(stream);
                }

                existingSanPham.HinhAnh = fileName;
            }

            // Cập nhật các thuộc tính khác
            existingSanPham.TenSanPham = sanPham.TenSanPham;
            existingSanPham.MoTa = sanPham.MoTa;
            existingSanPham.Gia = sanPham.Gia;
            existingSanPham.SoLuongTon = sanPham.SoLuongTon;
            existingSanPham.DanhMucId = sanPham.DanhMucId;
            existingSanPham.NhaCungCapId = sanPham.NhaCungCapId;
            // Thêm các thuộc tính khác nếu có

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.SanPhamId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DanhMucs = new SelectList(_context.DanhMucSanPhams, "DanhMucId", "TenDanhMuc");
            ViewBag.NhaCungCaps = new SelectList(_context.NhaCungCaps, "NhaCungCapId", "TenNhaCungCap");
            return View(sanPham);
        }


        // GET: AdminSanPham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.NhaCungCap)
                .FirstOrDefaultAsync(m => m.SanPhamId == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: AdminSanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.SanPhamId == id);
        }
        
        // search
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            var suggestions = await _context.SanPhams
                .Where(sp => sp.TenSanPham.Contains(term))
                .Select(sp => new
                {
                    sanPhamId = sp.SanPhamId,
                    tenSanPham = sp.TenSanPham,
                    hinhAnh = sp.HinhAnh,
                    soLuongTon = sp.SoLuongTon,
                    gia = sp.Gia
                })
                .Take(10)
                .ToListAsync();

            return Json(suggestions);
        }
    }
}