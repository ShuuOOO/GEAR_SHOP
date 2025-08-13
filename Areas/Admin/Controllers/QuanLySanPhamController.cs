using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Models.ViewModels;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class QuanLySanPhamController : Controller
    {
        private readonly _4tlShopContext _context;
        private readonly IWebHostEnvironment _env;

        public QuanLySanPhamController(_4tlShopContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/QuanLySanPham
        public async Task<IActionResult> Index([FromQuery] ProductFilterVM filter)
        {
            var query =
                from sp in _context.SanPhams
                join dm in _context.DanhMucSanPhams on sp.DanhMucId equals dm.DanhMucId into gdm
                from dm in gdm.DefaultIfEmpty()
                join ncc in _context.NhaCungCaps on sp.NhaCungCapId equals ncc.NhaCungCapId
                select new ProductListItemVM
                {
                    SanPhamID = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    Gia = sp.Gia,
                    GiaSauGiam = sp.GiaSauGiam,
                    SoLuongTon = sp.SoLuongTon,
                    HinhAnh = sp.HinhAnh,
                    TenDanhMuc = dm != null ? dm.TenDanhMuc : null,
                    TenNhaCungCap = ncc.TenNhaCungCap,
                    LaNoiBat = sp.LaNoiBat ?? false
                };

            if (!string.IsNullOrWhiteSpace(filter.q))
                query = query.Where(x => x.TenSanPham!.Contains(filter.q));

            if (filter.DanhMucID.HasValue)
                query = query.Where(x => x.TenDanhMuc != null &&
                                         _context.DanhMucSanPhams
                                            .Any(d => d.DanhMucId == filter.DanhMucID && d.TenDanhMuc == x.TenDanhMuc));

            if (filter.NhaCungCapID.HasValue)
            {
                // lọc theo tên ncc từ id (tối ưu hơn là join lại, nhưng giữ code gọn)
                var nccName = await _context.NhaCungCaps
                    .Where(n => n.NhaCungCapId == filter.NhaCungCapID)
                    .Select(n => n.TenNhaCungCap)
                    .FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(nccName))
                    query = query.Where(x => x.TenNhaCungCap == nccName);
            }

            if (filter.LaNoiBat.HasValue)
                query = query.Where(x => x.LaNoiBat == filter.LaNoiBat.Value);

            // Pagination
            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.SanPhamID)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            ViewBag.Filter = filter;
            ViewBag.Total = total;

            // DDL cho filter
            ViewBag.DanhMucs = await _context.DanhMucSanPhams
                .OrderBy(x => x.TenDanhMuc)
                .Select(x => new { x.DanhMucId, x.TenDanhMuc })
                .ToListAsync();

            ViewBag.NhaCungCaps = await _context.NhaCungCaps
                .OrderBy(x => x.TenNhaCungCap)
                .Select(x => new { x.NhaCungCapId, x.TenNhaCungCap })
                .ToListAsync();

            return View(items);
        }

        // GET: /Admin/QuanLySanPham/Create
        public async Task<IActionResult> Create()
        {
            await BuildFormViewBags();
            return View(new ProductFormVM());
        }

        // POST: /Admin/QuanLySanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormVM model, IFormFile? HinhAnhFile)
        {
            if (!ModelState.IsValid)
            {
                await BuildFormViewBags();
                return View(model);
            }

            var sp = new SanPham
            {
                TenSanPham = model.TenSanPham,
                MoTa = model.MoTa,
                Gia = model.Gia,
                SoLuongTon = model.SoLuongTon,
                HinhAnh = model.HinhAnh,
                DanhMucId = model.DanhMucID,
                NhaCungCapId = model.NhaCungCapID,
                LaNoiBat = model.LaNoiBat, // bool -> bool?
                ChiTiet = model.ChiTiet,
                GiaSauGiam = model.GiaSauGiam,
                ThongSoKyThuat = model.ThongSoKyThuat
            };

            if (HinhAnhFile != null && HinhAnhFile.Length > 0)
            {
                sp.HinhAnh = await SaveImageAsync(HinhAnhFile);
            }

            if (string.IsNullOrEmpty(sp.HinhAnh) == false && HinhAnhFile == null)
            {
                sp.HinhAnh = NormalizeImagePath(sp.HinhAnh);
            }

            try
            {
                _context.SanPhams.Add(sp);
                await _context.SaveChangesAsync();
                TempData["ok"] = "Tạo sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Đẩy lỗi ra view để biết vì sao không lưu được (FK, format, v.v.)
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                await BuildFormViewBags();
                return View(model);
            }
        }

        // GET: /Admin/QuanLySanPham/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            var model = new ProductFormVM
            {
                SanPhamID = sp.SanPhamId,
                TenSanPham = sp.TenSanPham,
                MoTa = sp.MoTa,
                Gia = sp.Gia,
                SoLuongTon = sp.SoLuongTon,
                HinhAnh = sp.HinhAnh,
                DanhMucID = sp.DanhMucId,
                NhaCungCapID = sp.NhaCungCapId,
                LaNoiBat = sp.LaNoiBat ?? false,
                ChiTiet = sp.ChiTiet,
                GiaSauGiam = sp.GiaSauGiam,
                ThongSoKyThuat = sp.ThongSoKyThuat
            };

            await BuildFormViewBags();
            return View(model);
        }

        // POST: /Admin/QuanLySanPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormVM model, IFormFile? HinhAnhFile)
        {
            if (id != model.SanPhamID) return BadRequest();

            if (!ModelState.IsValid)
            {
                await BuildFormViewBags();
                return View(model);
            }

            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            sp.TenSanPham = model.TenSanPham;
            sp.MoTa = model.MoTa;
            sp.Gia = model.Gia;
            sp.SoLuongTon = model.SoLuongTon;
            sp.DanhMucId = model.DanhMucID;
            sp.NhaCungCapId = model.NhaCungCapID;
            sp.LaNoiBat = model.LaNoiBat;
            sp.ChiTiet = model.ChiTiet;
            sp.GiaSauGiam = model.GiaSauGiam;
            sp.ThongSoKyThuat = model.ThongSoKyThuat;

            if (HinhAnhFile != null && HinhAnhFile.Length > 0)
            {
                sp.HinhAnh = await SaveImageAsync(HinhAnhFile);
            }

            if (HinhAnhFile == null && !string.IsNullOrWhiteSpace(sp.HinhAnh))
            {
                sp.HinhAnh = NormalizeImagePath(sp.HinhAnh);
            }

            await _context.SaveChangesAsync();
            TempData["ok"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/QuanLySanPham/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            _context.SanPhams.Remove(sp);
            await _context.SaveChangesAsync();
            TempData["ok"] = "Đã xóa sản phẩm.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/QuanLySanPham/ToggleFeatured/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFeatured(int id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            sp.LaNoiBat = !(sp.LaNoiBat ?? false);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task BuildFormViewBags()
        {
            ViewBag.DanhMucs = await _context.DanhMucSanPhams
                .OrderBy(x => x.TenDanhMuc)
                .Select(x => new { x.DanhMucId, x.TenDanhMuc })
                .ToListAsync();

            ViewBag.NhaCungCaps = await _context.NhaCungCaps
                .OrderBy(x => x.TenNhaCungCap)
                .Select(x => new { x.NhaCungCapId, x.TenNhaCungCap })
                .ToListAsync();
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath ?? "", "images", "products");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folder, fileName);
            using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }
            // đường dẫn tương đối để lưu DB
            return Path.Combine("images", "products", fileName).Replace("\\", "/");
        }
        private static string? NormalizeImagePath(string? p)
        {
            if (string.IsNullOrWhiteSpace(p)) return p;
            var path = p.Replace("\\", "/");
            return path.Contains('/') ? path : $"images/products/{path}";
        }
    }
}
