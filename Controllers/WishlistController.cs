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

        //  TIỆN ÍCH CHUNG 
        private async Task<int?> GetKhachHangIdAsync()
        {

            var _ = HttpContext.Session.Id;

            int? khachHangId = HttpContext.Session.GetInt32("KhachHangId");

            if (khachHangId == null && User.Identity.IsAuthenticated)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var taiKhoan = await _context.TaoTaiKhoans
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (taiKhoan == null)
                {
                    return null;  // không tìm thấy tài khoản → trả về null luôn
                }

                khachHangId = taiKhoan.KhachHangId;
                if (khachHangId.HasValue)
                {
                    HttpContext.Session.SetInt32("KhachHangId", khachHangId.Value);
                }  // đã chắc chắn có giá trị
            }

            return khachHangId;
        }

        private async Task<Wishlist> GetOrCreateWishlistAsync()
        {
            var sessionId = HttpContext.Session.Id;
            var khachHangId = await GetKhachHangIdAsync();

            var wishlist = await _context.Wishlists
    .Include(w => w.WishlistItems)
        .ThenInclude(i => i.SanPham)
    .FirstOrDefaultAsync(w =>
        (khachHangId != null && w.TaiKhoanId == khachHangId)
        || (khachHangId == null && w.TaiKhoanId == null && w.SessionId == sessionId));

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    TaiKhoanId = khachHangId,
                    SessionId = khachHangId == null ? sessionId : null
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            // Nếu đang đăng nhập mà wishlist vẫn còn sessionId → chuyển đổi
            if (wishlist.SessionId != null && khachHangId != null)
            {
                wishlist.TaiKhoanId = khachHangId;
                wishlist.SessionId = null;
                await _context.SaveChangesAsync();
            }

            return wishlist;
        }

        //  INDEX 
        public async Task<IActionResult> Index()
        {
            var wishlist = await GetOrCreateWishlistAsync();

            await _context.Entry(wishlist)
                .Collection(w => w.WishlistItems)
                .Query()
                .Include(i => i.SanPham)
                .LoadAsync();

            var items = wishlist.WishlistItems?.Where(i => i.SanPham != null).ToList()
                         ?? new List<WishlistItem>();

            return View(items);  // @model List<WishlistItem>
        }


        //  THÊM YÊU THÍCH 
        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var wishlist = await GetOrCreateWishlistAsync();

            wishlist.WishlistItems ??= new List<WishlistItem>();
            // Kiểm tra nếu sản phẩm đã có trong danh sách yêu thích
            if (!wishlist.WishlistItems.Any(i => i.SanPhamId == productId))
            {
                var item = new WishlistItem
                {
                    SanPhamId = productId,
                    WishlistId = wishlist.WishlistId
                };

                _context.WishlistItems.Add(item);
                await _context.SaveChangesAsync();
            }

            await _context.Entry(wishlist)
                .Collection(w => w.WishlistItems)
                .Query()
                .Include(wi => wi.SanPham)
                .LoadAsync();
            return RedirectToAction("Index");
        }

        //  AJAX THÊM YÊU THÍCH 
        [HttpPost]
        public async Task<IActionResult> AddToWishlistAjax(int productId)
        {
            // Lấy hoặc tạo wishlist
            var wishlist = await GetOrCreateWishlistAsync();


            // Luôn load lại danh sách WishlistItems
            await _context.Entry(wishlist)
                .Collection(w => w.WishlistItems)
                .Query()
                .Include(i => i.SanPham)
                .LoadAsync();

            // Đảm bảo không null
            wishlist.WishlistItems ??= new List<WishlistItem>();

            // Kiểm tra sản phẩm đã tồn tại chưa
            bool exists = wishlist.WishlistItems.Any(i => i.SanPhamId == productId);

            if (!exists)
            {
                var newItem = new WishlistItem
                {
                    SanPhamId = productId,
                    WishlistId = wishlist.WishlistId // BẮT BUỘC GÁN LẠI
                };

                _context.WishlistItems.Add(newItem);
                await _context.SaveChangesAsync();
            }

            // Load lại kèm sản phẩm
            await _context.Entry(wishlist)
                .Collection(w => w.WishlistItems)
                .Query()
                .Include(i => i.SanPham)
                .LoadAsync();

            int count = wishlist.WishlistItems.Count;

            return Json(new
            {
                success = true,
                message = "Đã thêm vào danh sách yêu thích!",
                wishlistCount = count
            });
        }


        // XOÁ 
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

        //  ĐẾM TRÁI TIM 
        public async Task<IActionResult> WishlistCount()
        {
            var TaikhoanID = await GetKhachHangIdAsync();
            var sessionId = HttpContext.Session.Id;

            var count = await _context.WishlistItems
                .Where(wi => _context.Wishlists
                    .Where(w => (TaikhoanID != null && w.TaiKhoanId == TaikhoanID) ||
                                (TaikhoanID == null && w.SessionId == sessionId))
                    .Select(w => w.WishlistId)
                    .Contains(wi.WishlistId))
                .CountAsync();

            return Json(new { count });
        }
    }
}
