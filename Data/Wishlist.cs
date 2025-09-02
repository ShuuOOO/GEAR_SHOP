using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class Wishlist
{
    public int WishlistId { get; set; }

<<<<<<< HEAD
    public int? TaiKhoanId { get; set; }

    public string? SessionId { get; set; }

    public virtual KhachHang? TaiKhoan { get; set; }

    public virtual TaoTaiKhoan? TaiKhoanNavigation { get; set; }

=======
    //public int? KhachHangId { get; set; }
    public string? SessionId { get; set; }
    public int? TaiKhoanId { get; set; }
    //public virtual KhachHang? KhachHang { get; set; }
    public virtual TaoTaiKhoan? TaoTaiKhoan { get; set; }
>>>>>>> aa1cdc1
    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
