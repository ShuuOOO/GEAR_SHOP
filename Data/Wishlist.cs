using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class Wishlist
{
    public int WishlistId { get; set; }

    //public int? KhachHangId { get; set; }
    public string? SessionId { get; set; }
    public int? TaiKhoanId { get; set; }
    //public virtual KhachHang? KhachHang { get; set; }
    public virtual TaoTaiKhoan? TaoTaiKhoan { get; set; }
    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
