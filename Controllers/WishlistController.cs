using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TL4_SHOP.Data;

namespace TL4_SHOP.Controllers
{
    public class WishlistController : BaseController
    {
        private readonly _4tlShopContext _context;

        public WishlistController(_4tlShopContext context) : base(context)
        {
            _context = context;
        }
        // Lấy TaiKhoanId từ session hoặc từ User.Identity
        private async Task<int?> GetTaiKhoanIdAsync()
        {
            int? taiKhoanId = HttpContext.Session.GetInt32("TaiKhoanId");

            if (taiKhoanId == null && User.Identity.IsAuthenticated)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var taiKhoan = await _context.TaoTaiKhoans
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (taiKhoan == null) return null;

                taiKhoanId = taiKhoan.TaiKhoanId;
                HttpContext.Session.SetInt32("TaiKhoanId", taiKhoanId.Value);
            }
            return taiKhoanId;
        }

        // Lấy hoặc tạo wishlist (tích hợp luôn merge khi login)
        private async Task<Wishlist> GetOrCreateWishlistAsync()
        {
            var sessionId = HttpContext.Session.Id;
            var taiKhoanId = await GetTaiKhoanIdAsync();

            Wishlist wishlist = null;

            if (taiKhoanId != null)
            {
                // Lấy wishlist theo tài khoản
                wishlist = await _context.Wishlists
                    .Include(w => w.WishlistItems)
                    .ThenInclude(i => i.SanPham)
                    .FirstOrDefaultAsync(w => w.TaiKhoanId == taiKhoanId);

                // Merge từ session (nếu có)
                var sessionWishlist = await _context.Wishlists
                    .Include(w => w.WishlistItems)
                    .FirstOrDefaultAsync(w => w.SessionId == sessionId && w.TaiKhoanId == null);

                if (sessionWishlist != null)
                {
                    if (wishlist == null)
                    {
                        // Gắn session wishlist sang tài khoản
                        sessionWishlist.TaiKhoanId = taiKhoanId;
                        sessionWishlist.SessionId = null;
                        wishlist = sessionWishlist;
                    }
                    else
                    {
                        // Merge sản phẩm
                        foreach (var item in sessionWishlist.WishlistItems)
                        {
                            if (!wishlist.WishlistItems.Any(x => x.SanPhamId == item.SanPhamId))
                            {
                                wishlist.WishlistItems.Add(new WishlistItem
                                {
                                    SanPhamId = item.SanPhamId
                                });
                            }
                        }
                        _context.Wishlists.Remove(sessionWishlist);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Vãng lai -> theo session
                wishlist = await _context.Wishlists
                    .Include(w => w.WishlistItems)
                    .ThenInclude(i => i.SanPham)
                    .FirstOrDefaultAsync(w => w.SessionId == sessionId);

                if (wishlist == null)
                {
                    wishlist = new Wishlist
                    {
                        SessionId = sessionId
                    };
                    _context.Wishlists.Add(wishlist);
                    await _context.SaveChangesAsync();
                }
            }
            return wishlist;
        }

        // Trang Index
        public async Task<IActionResult> Index()
        {
            var wishlist = await GetOrCreateWishlistAsync();
            var items = wishlist.WishlistItems?.Where(i => i.SanPham != null).ToList()
                         ?? new List<WishlistItem>();
            return View(items);
        }

        // Thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var taiKhoanId = HttpContext.Session.GetInt32("TaiKhoanId");
            if (taiKhoanId == null)
            {
                // Nếu chưa đăng nhập thì báo lỗi
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm vào danh sách yêu thích." });
            }

            // Lấy wishlist của tài khoản
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.TaiKhoanId == taiKhoanId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    TaiKhoanId = taiKhoanId.Value,
                    WishlistItems = new List<WishlistItem>()
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync(); // save để có WishlistId
            }

            // Kiểm tra sản phẩm đã có chưa
            if (!wishlist.WishlistItems.Any(x => x.SanPhamId == productId))
            {
                wishlist.WishlistItems.Add(new WishlistItem
                {
                    SanPhamId = productId,
                    TaiKhoanId = taiKhoanId.Value,   //  thêm tài khoản vào
                    WishlistId = wishlist.WishlistId //  gán luôn wishlist id
                });
            }
            else
            {
                return Json(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích." });
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã thêm vào yêu thích." });
        }

        // Thêm AJAX
        [HttpPost]
        public async Task<IActionResult> AddToWishlistAjax(int productId)
        {
            var taiKhoanId = HttpContext.Session.GetInt32("TaiKhoanId");
            if (taiKhoanId == null)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để thêm sản phẩm yêu thích." });
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.TaiKhoanId == taiKhoanId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    TaiKhoanId = taiKhoanId.Value,
                    WishlistItems = new List<WishlistItem>()
                };
                _context.Wishlists.Add(wishlist);
            }

            //  Kiểm tra sản phẩm đã có chưa
            if (wishlist.WishlistItems.Any(i => i.SanPhamId == productId))
            {
                int countExist = wishlist.WishlistItems.Count;
                return Json(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích!", wishlistCount = countExist });
            }

            // Nếu chưa có thì thêm
            wishlist.WishlistItems.Add(new WishlistItem { SanPhamId = productId });
            await _context.SaveChangesAsync();

            int count = wishlist.WishlistItems.Count;
            return Json(new { success = true, message = "Đã thêm vào yêu thích!", wishlistCount = count });
        }


        // Xoá
        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int itemId)
        {
            var item = await _context.WishlistItems.FindAsync(itemId);
            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Đếm trái tim
        public async Task<IActionResult> WishlistCount()
        {
            var wishlist = await GetOrCreateWishlistAsync();
            int count = wishlist.WishlistItems.Count;
            return Json(new { count });
        }
    }
}
