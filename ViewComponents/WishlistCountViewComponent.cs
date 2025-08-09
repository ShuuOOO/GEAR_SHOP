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

        var khachHangId = HttpContext.Session.GetInt32("KhachHangId");
        var sessionId = HttpContext.Session.Id;

        if (khachHangId != null)
        {
            // Người dùng đã đăng nhập
            count = await _context.WishlistItems
                .Include(x => x.Wishlist)
                .CountAsync(x => x.Wishlist.KhachHangId == khachHangId);
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
