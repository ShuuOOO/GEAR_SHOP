using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class WishlistItem
{
    public int WishlistItemId { get; set; }

    public int WishlistId { get; set; }

    public int SanPhamId { get; set; }

    public virtual SanPham SanPham { get; set; } = null!;

    public virtual Wishlist Wishlist { get; set; } = null!;
}
