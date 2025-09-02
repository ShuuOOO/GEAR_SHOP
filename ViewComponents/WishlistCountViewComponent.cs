using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

public class WishlistCountViewComponent : ViewComponent
{
    private readonly _4tlShopContext _context;

    public WishlistCountViewComponent(_4tlShopContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        int count = 0;

        var taiKhoanId = HttpContext.Session.GetInt32("TaiKhoanId");
        var sessionId = HttpContext.Session.Id;

        if (taiKhoanId != null)
        {
            // Người dùng đã đăng nhập
            count = await _context.WishlistItems
                .Include(x => x.Wishlist)
                .CountAsync(x => x.Wishlist.TaiKhoanId == taiKhoanId);
        }
        else
        {
            // Người dùng vãng lai
            count = await _context.WishlistItems
                .Include(x => x.Wishlist)
                .CountAsync(x => x.Wishlist.SessionId == sessionId);
        }

        return View(count);
    }
}
