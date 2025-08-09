using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class KhachHang
{
    public int KhachHangId { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public virtual ICollection<DiaChi> DiaChis { get; set; } = new List<DiaChi>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<TaoTaiKhoan> TaoTaiKhoans { get; set; } = new List<TaoTaiKhoan>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
