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

        var TaikhoanID = HttpContext.Session.GetInt32("TaikhoanID");
        var sessionId = HttpContext.Session.Id;

        if (TaikhoanID != null)
        {
            // Người dùng đã đăng nhập
            count = await _context.WishlistItems
                .Include(x => x.Wishlist)
                .CountAsync(x => x.Wishlist.TaiKhoanId == TaikhoanID);
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
