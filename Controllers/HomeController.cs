using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TL4_SHOP.Data;
using TL4_SHOP.Extensions;
using TL4_SHOP.Models;
using TL4_SHOP.Models.ViewModels;
using TL4_SHOP.Extensions;

namespace TL4_SHOP.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(_4tlShopContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];

            // Sản phẩm nổi bật
            var featuredProducts = await _context.SanPhams
                .Where(sp => sp.LaNoiBat == true)
                .OrderByDescending(sp => sp.SanPhamId)
                .Take(8)
                .ToListAsync();
            ViewBag.FeaturedProducts = featuredProducts;

            // 4 tin công nghệ mới nhất
            var latestNews = await _context.TechNews
                .AsNoTracking()
                .OrderByDescending(n => n.PublishedAt)
                .Take(4)
                .ToListAsync();
            ViewBag.LatestNews = latestNews;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Shop(string searchTerm, int? danhMucId, int? nhaCungCapId,
            decimal? minPrice, decimal? maxPrice, string sortBy, int page = 1)
        {
            var viewModel = new ShopViewModel
            {
                SearchTerm = searchTerm,
                DanhMucId = danhMucId,
                NhaCungCapId = nhaCungCapId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                CurrentPage = page
            };

            // Lọc sản phẩm
            var query = _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.NhaCungCap)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(s => s.TenSanPham.Contains(searchTerm));

            if (danhMucId.HasValue)
                query = query.Where(s => s.DanhMucId == danhMucId.Value);

            if (nhaCungCapId.HasValue)
                query = query.Where(s => s.NhaCungCapId == nhaCungCapId.Value);

            if (minPrice.HasValue)
                query = query.Where(s => s.Gia >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(s => s.Gia <= maxPrice.Value);

            // Sắp xếp
            query = query.OrderByDescending(sp => sp.GiaSauGiam < sp.Gia)
             .ThenBy(sp => sp.TenSanPham);

            // Phân trang
            viewModel.TotalItems = await query.CountAsync();
            viewModel.SanPhams = await query
                .Skip((page - 1) * viewModel.PageSize)
                .Take(viewModel.PageSize)
                .ToListAsync();

            // Gán danh mục và nhà cung cấp cho filter
            viewModel.DanhMucs = await _context.DanhMucSanPhams.ToListAsync();
            viewModel.NhaCungCaps = await _context.NhaCungCaps.ToListAsync();

            return View(viewModel);
        }

        public async Task<IActionResult> SearchSuggestions(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
                return Json(new List<object>());

            var suggestions = await _context.SanPhams
                .Where(s => s.TenSanPham.Contains(term))
                .Select(s => new
                {
                    s.SanPhamId,
                    TenSanPham = s.TenSanPham,
                    Gia = s.Gia,
                    HinhAnh = s.HinhAnh,
                    // [THÊM] URL ảnh tuyệt đối cho client
                    imageUrl = s.HinhAnh.StartsWith("http")
                        ? s.HinhAnh
                        : (s.HinhAnh.StartsWith("/") ? s.HinhAnh
                            : "/" + s.HinhAnh.Replace("~/", ""))
                })
                .ToListAsync();

            return Json(suggestions);
        }

        public IActionResult ShopDetail() => View();
        public IActionResult ShoppingCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("GioHang") ?? new List<CartItem>();
            return View("~/Views/Cart/ShoppingCart.cshtml", cart); // dùng đường dẫn tuyệt đối
        }
        public IActionResult Checkout() => View();
        public IActionResult Contact() => View();

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Json(new List<object>());

            var products = await _context.SanPhams
                .Where(s => s.TenSanPham.Contains(term))
                .Take(10)
                .Select(s => new
                {
                    id = s.SanPhamId,
                    name = s.TenSanPham,
                    price = s.Gia,
                    image = s.HinhAnh
                })
                .ToListAsync();

            return Json(products);
        }
    }
}
