using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TL4_SHOP.Data;

public partial class WishlistItem
{
    public int WishlistItemId { get; set; }

    public int WishlistId { get; set; }

    public int SanPhamId { get; set; }
    [ForeignKey(nameof(TaiKhoanId))]
    public int? TaiKhoanId { get; set; }
    public virtual TaoTaiKhoan? TaoTaiKhoan { get; set; }
    public virtual SanPham SanPham { get; set; } = null!;
    public virtual Wishlist Wishlist { get; set; } = null!;
}
