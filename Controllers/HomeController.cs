using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TL4_SHOP.Data;
using TL4_SHOP.Models;
using TL4_SHOP.Models.ViewModels;
using SanPhamModel = TL4_SHOP.Models.SanPham;


namespace TL4_SHOP.Controllers
{
    public class HomeController : Controller
    {
        private readonly _4tlShopContext _context;

        public HomeController(_4tlShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = User.Identity.Name;
            ViewBag.Message = TempData["Message"];
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

            // Lấy tất cả sản phẩm
            var query = _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.NhaCungCap)
                .AsQueryable();

            // Áp dụng bộ lọc tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => s.TenSanPham.Contains(searchTerm));
            }

            // Áp dụng bộ lọc danh mục
            if (danhMucId.HasValue)
            {
                query = query.Where(s => s.DanhMucId == danhMucId.Value);
            }

            // Áp dụng bộ lọc nhà cung cấp
            if (nhaCungCapId.HasValue)
            {
                query = query.Where(s => s.NhaCungCapId == nhaCungCapId.Value);
            }

            // Áp dụng bộ lọc giá
            if (minPrice.HasValue)
            {
                query = query.Where(s => s.Gia >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(s => s.Gia <= maxPrice.Value);
            }

            // Sắp xếp
            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(s => s.Gia);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(s => s.Gia);
                    break;
                case "name_asc":
                    query = query.OrderBy(s => s.TenSanPham);
                    break;
                default:
                    query = query.OrderBy(s => s.TenSanPham);
                    break;
            }

            // Tính tổng số sản phẩm
            viewModel.TotalItems = await query.CountAsync();

            // Phân trang
            viewModel.SanPhams = await query
                .Skip((page - 1) * viewModel.PageSize)
                .Take(viewModel.PageSize)
                .ToListAsync();

            // Load dữ liệu cho bộ lọc
            viewModel.DanhMucs = await _context.DanhMucSanPhams.ToListAsync();

            // Chuyển đổi từ Data.NhaCungCap sang Models.NhaCungCap
            var dataNhaCungCaps = await _context.NhaCungCaps.ToListAsync();
            viewModel.NhaCungCaps = dataNhaCungCaps.Select(ncc => new TL4_SHOP.Models.NhaCungCap
            {
                NhaCungCapId = ncc.NhaCungCapId,
                TenNhaCungCap = ncc.TenNhaCungCap,
                DiaChi = ncc.DiaChi,
                Phone = ncc.Phone,
                Email = ncc.Email
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> SearchSuggestions(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            var suggestions = await _context.SanPhams
                .Where(s => s.TenSanPham.Contains(term))
                .Select(s => new {
                    s.SanPhamId,
                    s.TenSanPham,
                    s.Gia,
                    s.HinhAnh
                })
                .ToListAsync();

            return Json(suggestions);
        }



        public IActionResult ShopDetail()
        {
            return View();
        }

        public IActionResult ShoppingCart()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        // API endpoint cho tìm kiếm AJAX
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

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